#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Text;
using System.Xml;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public abstract class XmlCodec : IMediaTypeReader, IMediaTypeWriter
    {
        Action<XmlWriterSettings> _configuration = config => { };

        protected XmlCodec()
        {
            Configuration = delegate { };
        }
        object ICodec.Configuration
        {
            get { return Configuration; }
            set
            {
                Configuration = value as Action<XmlWriterSettings>;
            }
        }

        protected XmlWriter Writer { get; private set; }

        protected Action<XmlWriterSettings> Configuration
        {
            get { return _configuration; }
            set
            {
                if (value == null) value = config => { };
                _configuration = value;
            }
        }

        public abstract void WriteToCore(object entity, IHttpEntity response);
        public abstract object ReadFrom(IHttpEntity request, IType destinationType, string memberName);

        public virtual void WriteTo(object entity, IHttpEntity response, string[] parameters)
        {
            var responseStream = response.Stream;
            var xmlSettings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                ConformanceLevel = ConformanceLevel.Document,
                Indent = true,
                NewLineOnAttributes = true,
                OmitXmlDeclaration = false,
                CloseOutput = true,
                CheckCharacters = false
            };
            if (response.Headers.ContentType == null)
                response.Headers.ContentType = new MediaType("application/xml;charset=utf-8");
            else if (response.Headers.ContentType.Matches(MediaType.Xml))
                response.Headers.ContentType.CharSet = "utf-8";
            Configuration(xmlSettings);

            using (Writer = XmlWriter.Create(responseStream, xmlSettings))
                WriteToCore(entity, response);
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion