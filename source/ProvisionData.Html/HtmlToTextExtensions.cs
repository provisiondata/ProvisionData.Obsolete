/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData.Extensions
{
	using HtmlAgilityPack;
	using System;
	using System.IO;
	using System.Text.RegularExpressions;

	public static class HtmlToTextExtensions
	{
		/// <summary>
		/// Converts valid HTML to plain text.
		/// </summary>
		/// <param name="html">well formed HTML</param>
		/// <returns>plain text</returns>
		public static String HtmlToText(this String html)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(html);
			return ConvertDoc(doc);
		}

		internal static String ConvertDoc(this HtmlDocument doc)
		{
			using (var sw = new StringWriter())
			{
				ConvertTo(doc.DocumentNode, sw);
				sw.Flush();
				return sw.ToString().Trim();
			}
		}

		internal static void ConvertContentTo(this HtmlNode node, TextWriter outText, PreceedingDomTextInfo textInfo)
		{
			foreach (var subnode in node.ChildNodes)
			{
				ConvertTo(subnode, outText, textInfo);
			}
		}

		internal static void ConvertTo(this HtmlNode node, TextWriter outText)
			=> ConvertTo(node, outText, new PreceedingDomTextInfo(false));

		internal static void ConvertTo(this HtmlNode node, TextWriter outText, PreceedingDomTextInfo textInfo)
		{
			String html;
			switch (node.NodeType)
			{
				case HtmlNodeType.Comment:
					// don't output comments
					break;
				case HtmlNodeType.Document:
					ConvertContentTo(node, outText, textInfo);
					break;
				case HtmlNodeType.Text:
					// script and style must not be output
					var parentName = node.ParentNode.Name;
					if ((parentName == "script") || (parentName == "style"))
					{
						break;
					}
					// get text
					html = ((HtmlTextNode)node).Text;
					// is it in fact a special closing node output as text?
					if (HtmlNode.IsOverlappedClosingElement(html))
					{
						break;
					}
					// check the text is meaningful and not a bunch of whitespaces
					if (html.Length == 0)
					{
						break;
					}
					if (!textInfo.WritePrecedingWhiteSpace || textInfo.LastCharWasSpace)
					{
						html = html.TrimStart();
						if (html.Length == 0)
						{ break; }
						textInfo.IsFirstTextOfDocWritten.Value = textInfo.WritePrecedingWhiteSpace = true;
					}
					outText.Write(HtmlEntity.DeEntitize(Regex.Replace(html.TrimEnd(), @"\s{2,}", " ")));

					// Index operators aren't compatible with net48
					if (textInfo.LastCharWasSpace = Char.IsWhiteSpace(html[html.Length - 1]))
					{
						outText.Write(' ');
					}

					break;
				case HtmlNodeType.Element:
					String endElementString = null;
					Boolean isInline;
					var skip = false;
					var listIndex = 0;
					switch (node.Name)
					{
						case "nav":
							skip = true;
							isInline = false;
							break;
						case "body":
						case "section":
						case "article":
						case "aside":
						case "h1":
						case "h2":
						case "header":
						case "footer":
						case "address":
						case "main":
						case "div":
						case "p": // stylistic - adjust as you tend to use
							if (textInfo.IsFirstTextOfDocWritten)
							{
								outText.Write("\r\n");
							}
							endElementString = "\r\n";
							isInline = false;
							break;
						case "br":
							outText.Write("\r\n");
							skip = true;
							textInfo.WritePrecedingWhiteSpace = false;
							isInline = true;
							break;
						case "a":
							if (node.Attributes.Contains("href"))
							{
								var href = node.Attributes["href"].Value.Trim();
								if (node.InnerText.IndexOf(href, StringComparison.InvariantCultureIgnoreCase) == -1)
								{
									endElementString = "<" + href + ">";
								}
							}
							isInline = true;
							break;
						case "li":
							if (textInfo.ListIndex > 0)
							{
								outText.Write("\r\n{0}. ", textInfo.ListIndex++);
							}
							else
							{
								outText.Write("\r\n* "); //using '*' as bullet char, with tab after, but whatever you want eg "\t->", if utf-8 0x2022
							}
							isInline = false;
							break;
						case "ol":
							listIndex = 1;
							goto case "ul";
						case "ul": //not handling nested lists any differently at this stage - that is getting close to rendering problems
							endElementString = "\r\n";
							isInline = false;
							break;
						case "img": //inline-block in reality
							if (node.Attributes.Contains("alt"))
							{
								outText.Write('[' + node.Attributes["alt"].Value);
								endElementString = "]";
							}
							if (node.Attributes.Contains("src"))
							{
								outText.Write('<' + node.Attributes["src"].Value + '>');
							}
							isInline = true;
							break;
						default:
							isInline = true;
							break;
					}
					if (!skip && node.HasChildNodes)
					{
						ConvertContentTo(node, outText, isInline ? textInfo : new PreceedingDomTextInfo(textInfo.IsFirstTextOfDocWritten) { ListIndex = listIndex });
					}
					if (endElementString != null)
					{
						outText.Write(endElementString);
					}
					break;
			}
		}
	}

	internal class PreceedingDomTextInfo
	{
		public PreceedingDomTextInfo(BoolWrapper isFirstTextOfDocWritten)
		{
			IsFirstTextOfDocWritten = isFirstTextOfDocWritten;
		}

		public Boolean WritePrecedingWhiteSpace { get; set; }
		public Boolean LastCharWasSpace { get; set; }
		public BoolWrapper IsFirstTextOfDocWritten { get; }
		public Int32 ListIndex { get; set; }
	}

	internal class BoolWrapper
	{
		public BoolWrapper() { }
		public Boolean Value { get; set; }

		public static implicit operator Boolean(BoolWrapper boolWrapper) => boolWrapper.Value;

		public static implicit operator BoolWrapper(Boolean boolWrapper) => new() { Value = boolWrapper };
	}
}
