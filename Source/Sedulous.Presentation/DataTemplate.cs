using System;
using System.Globalization;
using System.Xml.Linq;
using Sedulous.Core;
using Sedulous.Presentation.Uvml;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Describes the visual structure of a data object.
    /// </summary>
    [UvmlKnownType]
    public class DataTemplate : FrameworkTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplate"/> class.
        /// </summary>
        /// <param name="template">The UVML template that this instance represents.</param>
        /// <param name="dataSourceWrapperName">The name of the template's data source wrapper type.</param>
        public DataTemplate(UvmlTemplate template, String dataSourceWrapperName)
            : base(template, dataSourceWrapperName)
        { }

        /// <summary>
        /// Loads a new instance of the <see cref="DataTemplate"/> class from the specified UVML element.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="root">The root element of the UVML template to load.</param>
        /// <param name="cultureInfo">The <see cref="CultureInfo"/> to use when parsing values.</param>
        /// <returns>The <see cref="DataTemplate"/> instance that was created from the specified UVML element.</returns>
        public static DataTemplate FromUvml(FrameworkContext context, XElement root, CultureInfo cultureInfo = null)
        {
            Contract.Require(context, nameof(context));
            Contract.Require(root, nameof(root));

            var dataSourceAnnotation = root.Parent?.Annotation<FrameworkTemplateNameAnnotation>();
            var dataSourceWrapperName = dataSourceAnnotation?.Name;

            var templatedParentType = default(Type);

            var templatedObjectName = root.Name.LocalName;
            var templatedObjectType = default(Type);

            if (!context.GetUI().GetPresentationFoundation().GetKnownType(templatedObjectName, out templatedObjectType))
                throw new UvmlException(PresentationStrings.UnrecognizedType.Format(templatedObjectName));

            var template = UvmlLoader.CreateTemplateFromXml(context, root, templatedParentType, templatedObjectType, cultureInfo);
            var instance = new DataTemplate(template, dataSourceWrapperName);

            return instance;
        }
    }
}
