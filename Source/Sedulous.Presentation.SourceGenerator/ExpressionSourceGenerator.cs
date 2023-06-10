using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sedulous.Presentation.SourceGenerator
{
    /// <summary>
    /// 
    /// </summary>
    [Generator]
    [CLSCompliant(false)]
    public class ExpressionSourceGenerator : ISourceGenerator
    {
        /// <summary>
        /// Gets the namespace that contains data source wrappers for views.
        /// </summary>
        public static String DataSourceWrapperNamespaceForViews
        {
            get { return "Sedulous.Presentation.CompiledExpressions"; }
        }

        /// <summary>
        /// Gets the namespace that contains data source wrappers for component templates.
        /// </summary>
        public static String DataSourceWrapperNamespaceForComponentTemplates
        {
            get { return "Sedulous.Presentation.CompiledExpressions"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(GeneratorExecutionContext context)
        {
            var source = """
                public class TestGenClass{
                    public static void SayHello(){
                        System.Console.WriteLine("Hello");
                    }
                }
                """;
            context.AddSource("test.gen.cs", source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
