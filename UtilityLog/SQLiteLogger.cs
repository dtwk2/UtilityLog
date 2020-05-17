using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Splat;
using SQLite;

namespace UtilityLog
{
    public class SQLiteLogger : IEnableLogger
    {
        private readonly Subject<Unit> isInitialised = new Subject<Unit>();

        public SQLiteLogger(SQLiteAsyncConnection sqliteConnection)
        {

            var obs = sqliteConnection.CreateTableAsync<Log>()
                .ToObservable();

            _ = ObservableLogger
                .Instance
                .Messages
                .CombineLatest(obs, RunCount(sqliteConnection, obs), (a, b, c) => (a.level, a.message, b, c))
                .Subscribe(async c =>
                {
                    var (level, message, result, runCount) = c;
                    try
                    {
                        if (result == CreateTableResult.Created)
                            this.Log().Info($"Table {nameof(Log)} has been {result.ToString()}.");
                        await sqliteConnection.InsertAsync(new Log(message, level, runCount));
                        isInitialised.OnNext(Unit.Default);
                        isInitialised.OnCompleted();
                    }
                    catch (Exception ex)
                    {

                    }
                });


            this
                .Log()
                .Info($"{nameof(SQLiteLogger)} Initialized.");
        }

        public SQLiteLogger(SQLiteConnection sqliteConnection)
        {
            sqliteConnection.CreateTable<Log>();

            var runCount = RunCount(sqliteConnection);

            _ = ObservableLogger
                .Instance
                .Messages
                .Subscribe(async c =>
                {
                    var (level, message) = c;
                    try
                    {
                        sqliteConnection.Insert(new Log(message, level, runCount));
                        isInitialised.OnNext(Unit.Default);
                        isInitialised.OnCompleted();
                    }
                    catch (Exception ex)
                    {

                    }
                });

            this
                .Log()
                .Info($"{nameof(SQLiteLogger)} Initialized.");
        }

        public IObservable<Unit> IsInitialised => isInitialised;

        private int RunCount(SQLiteConnection sqliteConnection)
        {
            return sqliteConnection.FindWithQuery<Runcount>($"SELECT MAX({nameof(Log.RunCount)}) as {nameof(Runcount.Value)} FROM {nameof(Log)}").Value ?? 0 + 1;
        }

        private IObservable<int> RunCount(SQLiteAsyncConnection sqliteConnection, IObservable<CreateTableResult> observable)
        {
            return observable
                .SelectMany(a => sqliteConnection.FindWithQueryAsync<Runcount>($"SELECT MAX({nameof(Log.RunCount)}) as {nameof(Runcount.Value)} FROM {nameof(Log)}"))
                .Select(a => a.Value ?? 0)
                .Select(a => a + 1);
        }
        class Runcount
        {
            public int? Value { get; set; }
        }
    }
}
