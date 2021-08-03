// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Pcs.Hfrr.Log
{
    /// <summary>
    /// A logger which will send messages to the console.
    /// </summary>
    public class ConsoleLogger : ObservableLogger
    {
        public ConsoleLogger()
        {
            _ = Messages
            .Subscribe(a =>
            {
                var (ad, c,_) = a;

                if (c is string msg)
                    Console.WriteLine(msg);
                if (c is Exception exception)
                {
                    while (exception != null)
                    {
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
                        exception = exception.InnerException;
                    }
                }
                else
                    Console.WriteLine(c.ToString());
            });
        }
    }
}