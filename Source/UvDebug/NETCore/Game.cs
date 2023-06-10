using Sedulous;
using Sedulous.Presentation;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UvDebug
{
    public class Program
    {
        /// <summary>
        /// The application's entry point.
        /// </summary>
        /// <param name="args">An array containing the application's command line arguments.</param>
        public static void Main(String[] args)
        {
            using (var game = new Game())
            {
                game.resolveContent = args.Contains("-resolve:content");
                game.compileContent = args.Contains("-compile:content");
                game.compileExpressions = args.Contains("-compile:expressions");

                game.Run();
            }
        }
    }
}
