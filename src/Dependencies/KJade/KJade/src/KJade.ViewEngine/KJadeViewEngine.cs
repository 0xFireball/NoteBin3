using KJade.Compiler;
using KJade.Compiler.Html;
using Nancy;
using Nancy.Responses;
using Nancy.ViewEngines;
using Nancy.ViewEngines.SuperSimpleViewEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KJade.ViewEngine
{
    public class KJadeViewEngine : IViewEngine
    {
        private readonly SuperSimpleViewEngine engineWrapper;

        public KJadeViewEngine(SuperSimpleViewEngine engineWrapper)
        {
            this.engineWrapper = engineWrapper;
        }

        public IEnumerable<string> Extensions => new[]
        {
            "jade",
            "kjade",
            "kade",
        };

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            //Nothing to really do here
        }

        private static readonly Regex ImportRegex = new Regex(@"@import\s(?<ViewName>[\w/.]+)", RegexOptions.Compiled);

        private static readonly Regex ConditionalRegex = new Regex(@"@if(?<Not>not)?(?<AllowNonexistent>\?)?\smodel(?:\.(?<ParameterName>[a-zA-Z0-9-_]+)+)?(?<Contents>[\s\S]*?)@endif", RegexOptions.Compiled);

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            /*
            var response = new HtmlResponse();
            var html = renderContext.ViewCache.GetOrAdd(viewLocationResult, result =>
            {
                return EvaluateKJade(viewLocationResult, model, renderContext);
            });

            var renderedHtml = html;

            response.Contents = stream =>
            {
                var writer = new StreamWriter(stream);
                writer.Write(renderedHtml);
                writer.Flush();
            };

            return response;
            */
            return new HtmlResponse(contents: s =>
            {
                var writer = new StreamWriter(s);
                var templateContents = renderContext.ViewCache.GetOrAdd(viewLocationResult, vr =>
                {
                    using (var reader = vr.Contents.Invoke())
                        return reader.ReadToEnd();
                });

                var renderedHtml = EvaluateKJade(viewLocationResult, model, renderContext);
                writer.Write(renderedHtml);
                writer.Flush();
            });
        }

        private string ReadView(ViewLocationResult locationResult)
        {
            string content;
            using (var reader = locationResult.Contents.Invoke())
            {
                content = reader.ReadToEnd();
            }
            return content;
        }

        private string PreprocessKJade(string kjade, object model, IRenderContext renderContext)
        {
            //Recursively replace @import
            kjade = ImportRegex.Replace(kjade, m =>
            {
                var partialViewName = m.Groups["ViewName"].Value;
                var partialModel = model;
                return PreprocessKJade(ReadView(renderContext.LocateView(partialViewName, partialModel)), model, renderContext);
            });

            //Process conditionals
            kjade = ConditionalRegex.Replace(kjade, m =>
            {
                var properties = ModelReflectionUtil.GetCaptureGroupValues(m, "ParameterName");
                var propertyVal = ModelReflectionUtil.GetPropertyValueFromParameterCollection(model, properties);
                var allowNonexistent = m.Groups["AllowNonexistent"].Success;
                var negateResult = m.Groups["Not"].Success;

                if (!propertyVal.Item1 && !allowNonexistent)
                {
                    return "[ERR!]";
                }

                bool evaluateResult = propertyVal.Item2 != null;

                //Check if property result is a BOOLEAN
                if (evaluateResult && propertyVal.Item2 is bool?)
                {
                    var booleanPropertyResult = propertyVal.Item2 as bool?;
                    evaluateResult = (bool)booleanPropertyResult;
                }

                if (negateResult)
                {
                    evaluateResult = !evaluateResult;
                    //We don't want them both true, as that will mean:
                    //When we're looking for false, but allowing nonexistent,
                    //true will be output, but negate will make it false :(
                    if (!negateResult || !allowNonexistent)
                    {
                        evaluateResult = !evaluateResult;
                    }
                }

                var conditionalContent = m.Groups["Contents"].Value;

                if (evaluateResult)
                {
                    return conditionalContent;
                }
                return string.Empty;
            });

            var jadeCompiler = new JadeHtmlCompiler();
            return jadeCompiler.ReplaceInput(kjade, model);
        }

        private string EvaluateKJade(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            string content = ReadView(viewLocationResult);

            content = PreprocessKJade(content, model, renderContext);

            var jadeCompiler = new JadeHtmlCompiler();
            var compiledHtml = jadeCompiler.Compile(content, model);
            return compiledHtml.Value.ToString();
        }
    }
}