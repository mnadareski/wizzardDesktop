﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace SabreTools.Skippers
{
    /// <remarks>
    /// It is well worth considering just moving the XML files to code, similar to how RV does it
    /// if only because nobody really has any skippers outside of this. It would also make the
    /// output directory cleaner and less prone to user error in case something didn't get copied
    /// correctly. The contents of these files should still be added to the wiki, in that case.
    /// </remarks>
    public class SkipperFile
    {
        #region Fields

        /// <summary>
        /// Skipper name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Author names
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// File version
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Set of all rules in the skipper
        /// </summary>
        public List<SkipperRule> Rules { get; set; } = new List<SkipperRule>();

        /// <summary>
        /// Filename the skipper lives in
        /// </summary>
        public string SourceFile { get; set; } = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Create an empty SkipperFile object
        /// </summary>
        public SkipperFile() { }

        /// <summary>
        /// Create a SkipperFile object parsed from an input file
        /// </summary>
        /// <param name="filename">Name of the file to parse</param>
        public SkipperFile(string filename)
        {
            Rules = new List<SkipperRule>();
            SourceFile = Path.GetFileNameWithoutExtension(filename);

            XmlReader xtr = XmlReader.Create(filename, new XmlReaderSettings
            {
                CheckCharacters = false,
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                ValidationFlags = XmlSchemaValidationFlags.None,
                ValidationType = ValidationType.None,
            });

            bool valid = Parse(xtr);

            // If we somehow have an invalid file, zero out the fields
            if (!valid)
            {
                Name = null;
                Author = null;
                Version = null;
                Rules = null;
                SourceFile = null;
            }
        }

        #endregion

        #region Parsing Helpers

        /// <summary>
        /// Parse an XML document in as a SkipperFile
        /// </summary>
        /// <param name="xtr">XmlReader representing the document</param>
        /// <returns>True if the file could be parsed, false otherwise</returns>
        private bool Parse(XmlReader xtr)
        {
            if (xtr == null)
                return false;

            try
            {
                bool valid = false;
                xtr.MoveToContent();
                while (!xtr.EOF)
                {
                    if (xtr.NodeType != XmlNodeType.Element)
                        xtr.Read();

                    switch (xtr.Name.ToLowerInvariant())
                    {
                        case "detector":
                            valid = true;
                            xtr.Read();
                            break;

                        case "name":
                            Name = xtr.ReadElementContentAsString();
                            break;

                        case "author":
                            Author = xtr.ReadElementContentAsString();
                            break;

                        case "version":
                            Version = xtr.ReadElementContentAsString();
                            break;

                        case "rule":
                            SkipperRule rule = ParseRule(xtr);
                            if (rule != null)
                                Rules.Add(rule);
                            
                            xtr.Read();
                            break;

                        default:
                            xtr.Read();
                            break;
                    }
                }

                return valid;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parse an XML document in as a SkipperRule
        /// </summary>
        /// <param name="xtr">XmlReader representing the document</param>
        /// <returns>Filled SkipperRule on success, null otherwise</returns>
        private SkipperRule ParseRule(XmlReader xtr)
        {
            if (xtr == null)
                return null;

            try
            {
                // Get the information from the rule first
                SkipperRule rule = new SkipperRule
                {
                    StartOffset = null,
                    EndOffset = null,
                    Operation = HeaderSkipOperation.None,
                    Tests = new List<SkipperTest>(),
                    SourceFile = this.SourceFile,
                };

                string startOffset = xtr.GetAttribute("start_offset");
                if (startOffset != null)
                {
                    if (startOffset.ToLowerInvariant() == "eof")
                        rule.StartOffset = null;
                    else
                        rule.StartOffset = Convert.ToInt64(startOffset, 16);
                }

                string endOffset = xtr.GetAttribute("end_offset");
                if (endOffset != null)
                {
                    if (endOffset.ToLowerInvariant() == "eof")
                        rule.EndOffset = null;
                    else
                        rule.EndOffset = Convert.ToInt64(endOffset, 16);
                }

                string operation = xtr.GetAttribute("operation");
                if (operation != null)
                {
                    switch (operation.ToLowerInvariant())
                    {
                        case "bitswap":
                            rule.Operation = HeaderSkipOperation.Bitswap;
                            break;
                        case "byteswap":
                            rule.Operation = HeaderSkipOperation.Byteswap;
                            break;
                        case "wordswap":
                            rule.Operation = HeaderSkipOperation.Wordswap;
                            break;
                    }
                }

                // Now read the individual tests into the Rule
                XmlReader subreader = xtr.ReadSubtree();
                if (subreader != null)
                {
                    subreader.MoveToContent();
                    while (!subreader.EOF)
                    {
                        if (subreader.NodeType != XmlNodeType.Element)
                            subreader.Read();

                        switch (xtr.Name.ToLowerInvariant())
                        {
                            case "data":
                            case "or":
                            case "xor":
                            case "and":
                            case "file":
                                SkipperTest test = ParseTest(subreader);
                                if (test != null)
                                    rule.Tests.Add(test);

                                subreader.Read();
                                break;
                                
                            default:
                                subreader.Read();
                                break;
                        }
                    }
                }

                return rule;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parse an XML document in as a SkipperTest
        /// </summary>
        /// <param name="xtr">XmlReader representing the document</param>
        /// <returns>Filled SkipperTest on success, null otherwise</returns>
        private SkipperTest ParseTest(XmlReader xtr)
        {
            if (xtr == null)
                return null;

            try
            {
                // Get the test type
                SkipperTest test = new SkipperTest
                {
                    Offset = 0,
                    Value = new byte[0],
                    Result = true,
                    Mask = new byte[0],
                    Size = 0,
                    Operator = HeaderSkipTestFileOperator.Equal,
                };

                switch (xtr.Name.ToLowerInvariant())
                {
                    case "data":
                        test.Type = HeaderSkipTest.Data;
                        break;

                    case "or":
                        test.Type = HeaderSkipTest.Or;
                        break;

                    case "xor":
                        test.Type = HeaderSkipTest.Xor;
                        break;

                    case "and":
                        test.Type = HeaderSkipTest.And;
                        break;

                    case "file":
                        test.Type = HeaderSkipTest.File;
                        break;

                    default:
                        return null;
                }

                // Now populate all the parts that we can
                if (xtr.GetAttribute("offset") != null)
                {
                    string offset = xtr.GetAttribute("offset");
                    if (offset.ToLowerInvariant() == "eof")
                        test.Offset = null;
                    else
                        test.Offset = Convert.ToInt64(offset, 16);
                }

                if (xtr.GetAttribute("value") != null)
                {
                    string value = xtr.GetAttribute("value");

                    // http://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
                    test.Value = new byte[value.Length / 2];
                    for (int index = 0; index < test.Value.Length; index++)
                    {
                        string byteValue = value.Substring(index * 2, 2);
                        test.Value[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }
                }

                if (xtr.GetAttribute("result") != null)
                {
                    string result = xtr.GetAttribute("result");
                    if (!bool.TryParse(result, out bool resultBool))
                        resultBool = true;

                    test.Result = resultBool;
                }

                if (xtr.GetAttribute("mask") != null)
                {
                    string mask = xtr.GetAttribute("mask");

                    // http://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
                    test.Mask = new byte[mask.Length / 2];
                    for (int index = 0; index < test.Mask.Length; index++)
                    {
                        string byteValue = mask.Substring(index * 2, 2);
                        test.Mask[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }
                }

                if (xtr.GetAttribute("size") != null)
                {
                    string size = xtr.GetAttribute("size");
                    if (size.ToLowerInvariant() == "po2")
                        test.Size = null;
                    else
                        test.Size = Convert.ToInt64(size, 16);
                }

                if (xtr.GetAttribute("operator") != null)
                {
                    string oper = xtr.GetAttribute("operator");
                    test.Operator = oper.ToLowerInvariant() switch
                    {
                        "less" => HeaderSkipTestFileOperator.Less,
                        "greater" => HeaderSkipTestFileOperator.Greater,
                        "equal" => HeaderSkipTestFileOperator.Equal,
                        _ => HeaderSkipTestFileOperator.Equal,
                    };
                }

                return test;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Matching

        /// <summary>
        /// Get the SkipperRule associated with a given stream
        /// </summary>
        /// <param name="input">Stream to be checked</param>
        /// <param name="skipperName">Name of the skipper to be used, blank to find a matching skipper</param>
        /// <returns>The SkipperRule that matched the stream, null otherwise</returns>
        public SkipperRule GetMatchingRule(Stream input, string skipperName)
        {
            // If we have no name supplied, try to blindly match
            if (string.IsNullOrWhiteSpace(skipperName))
                return GetMatchingRule(input);

            // If the name matches the internal name of the skipper
            else if (string.Equals(skipperName, Name, StringComparison.OrdinalIgnoreCase))
                return GetMatchingRule(input);

            // If the name matches the source file name of the skipper
            else if (string.Equals(skipperName, SourceFile, StringComparison.OrdinalIgnoreCase))
                return GetMatchingRule(input);

            // Otherwise, nothing matches by default
            return null;
        }

        /// <summary>
        /// Get the matching SkipperRule from all Rules, if possible
        /// </summary>
        /// <param name="input">Stream to be checked</param>
        /// <returns>The SkipperRule that matched the stream, null otherwise</returns>
        private SkipperRule GetMatchingRule(Stream input)
        {
            // Loop through the rules until one is found that works
            foreach (SkipperRule rule in Rules)
            {
                // Always reset the stream back to the original place
                input.Seek(0, SeekOrigin.Begin);

                // If all tests in the rule pass, we return this rule
                if (rule.PassesAllTests(input))
                    return rule;
            }

            // If nothing passed, we return null by default
            return null;
        }

        #endregion
    }
}
