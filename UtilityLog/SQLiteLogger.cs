using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Splat;
using SQLite;

namespace UtilityLog
{
    public class SQLiteLogger : ObservableLogger
    {
        private static readonly string statement = $"SELECT MAX({nameof(Log.RunCount)}) as {nameof(Runcount.Value)} FROM {nameof(Log)}";

        private readonly Subject<Unit> isInitialised = new Subject<Unit>();

        public SQLiteLogger(SQLiteAsyncConnection sqliteAsyncConnection)
        {

            var obs = sqliteAsyncConnection.CreateTableAsync<Log>()
                .ToObservable();

            _ = Messages
                .CombineLatest(obs, RunCount(sqliteAsyncConnection, obs), (a, b, c) => (a.level, a.message, b, c))
                .Subscribe(async c =>
                {
                    var (level, message, result, runCount) = c;
                    try
                    {
                        var guid = Guid.NewGuid();

                        if (result == CreateTableResult.Created)
                            this.Write($"Table {nameof(Log)} has been {result}.", LogLevel.Info);
                        if (message is string msg)
                            await sqliteAsyncConnection.InsertAsync(new Log(msg, level, runCount) { Key = guid });
                        if (message is Exception exception)
                        {
                            while (exception != null)
                            {
                                await sqliteAsyncConnection.InsertAsync(new Log(exception, level, runCount) { Key = guid });
                                exception = exception.InnerException;
                            }
                        }

                        isInitialised.OnNext(Unit.Default);
                        isInitialised.OnCompleted();
                    }
                    catch (System.Exception ex)
                    {

                    }
                });


            this.Write($"{nameof(SQLiteLogger)} Initialized.", LogLevel.Info);
        }

        public SQLiteLogger(SQLiteConnection sqliteConnection)
        {
            sqliteConnection.CreateTable<Log>();

            var runCount = RunCount(sqliteConnection);

            _ = Messages
                .Subscribe(c =>
                {
                    var (level, message) = c;
                    try
                    {
                        var guid = Guid.NewGuid();
                        if (message is string msg)
                            sqliteConnection.Insert(new Log(msg, level, runCount) { Key = guid });
                        if (message is System.Exception exception)
                        {
                            while (exception != null)
                            {
                                sqliteConnection.Insert(new Log(exception, level, runCount) { Key = guid });
                                exception = exception.InnerException;
                            }
                        }
     
                        isInitialised.OnNext(Unit.Default);
                        isInitialised.OnCompleted();
                    }
                    catch (Exception ex)
                    {

                    }
                });

            this.Write($"{nameof(SQLiteLogger)} Initialized.", LogLevel.Info);
        }

        public IObservable<Unit> IsInitialised => isInitialised;

        private int RunCount(SQLiteConnection sqliteConnection)
        {
            var ss = sqliteConnection.FindWithQuery<Runcount>(statement).Value;
            return (ss ?? 0) + 1;
        }

        private IObservable<int> RunCount(SQLiteAsyncConnection sqliteConnection, IObservable<CreateTableResult> observable)
        {
            return observable
                .SelectMany(a => sqliteConnection.FindWithQueryAsync<Runcount>(statement))
                .Select(a => a.Value ?? 0)
                .Select(a => a + 1);
        }
        class Runcount
        {
            public int? Value { get; set; }
        }
    }
}
