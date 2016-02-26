﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bramble.Core
{
    public static class PropertyBagParser
    {
        public static IEnumerable<string> StripEmptyLines(IEnumerable<string> lines)
        {
            if (lines == null) throw new ArgumentNullException("lines");

            return lines.Where(line => line.Trim().Length > 0);
        }

        public static IEnumerable<string> StripComments(IEnumerable<string> lines)
        {
            if (lines == null) throw new ArgumentNullException("lines");

            return lines.Select(line => sCommentRegex.Match(line).Groups["content"].Value);
        }

        public static IEnumerable<string> ParseIncludes(string basePath, IEnumerable<string> lines)
        {
            if (basePath == null) throw new ArgumentNullException("basePath");
            if (lines == null) throw new ArgumentNullException("lines");

            foreach (string line in lines)
            {
                Match match = sIncludeRegex.Match(line);

                if (match.Success)
                {
                    // got an include
                    string path = match.Groups["path"].Value;
                    path = Path.Combine(basePath, path);

                    // see if it's a dir
                    if (Directory.Exists(path))
                    {
                        foreach (string filePath in Directory.GetFiles(path))
                        {
                            string[] includeLines = File.ReadAllLines(filePath);
                            foreach (string includeLine in ParseIncludes(path, includeLines))
                            {
                                yield return includeLine;
                            }
                        }
                    }
                    else if (File.Exists(path))
                    {
                        // it's a file
                        string[] includeLines = File.ReadAllLines(path);
                        foreach (string includeLine in ParseIncludes(Path.GetDirectoryName(path), includeLines))
                        {
                            yield return includeLine;
                        }
                    }
                    else
                    {
                        // it doesn't exist, do nothing
                        Console.WriteLine("Couldn't find include " + path);
                    }
                }
                else
                {
                    // not an include line, so just forward it
                    yield return line;
                }
            }
        }

        public static PropertyBag Parse(IndentationTree tree)
        {
            if (tree == null) throw new ArgumentNullException("tree");

            PropertyBag root = new PropertyBag(String.Empty, String.Empty);

            Stack<PropertyBag> parents = new Stack<PropertyBag>();
            parents.Push(root);

            ParseTree(tree, parents, new Stack<PropertyBag>());

            return root;
        }

        private static void ParseTree(IndentationTree tree, Stack<PropertyBag> parents, Stack<PropertyBag> abstractProps)
        {
            // temporary container for abstract properties. they can be
            // inherited from, but will not themselves be directly
            // added to the property tree
            abstractProps.Push(new PropertyBag("abstract"));

            foreach (IndentationTree child in tree.Children)
            {
                bool isAbstract = false;
                string name = parents.Peek().Count.ToString(); // default the property
                List<string> inherits = new List<string>();
                bool hasEquals = false;
                bool hasValue = false;
                string value = String.Empty;

                // parse the line
                Match match = sLineRegex.Match(child.Text);

                isAbstract = match.Groups["abstract"].Success;

                if (match.Groups["name"].Success)
                {
                    name = match.Groups["name"].Value.Trim();
                }

                if (match.Groups["inherits"].Success)
                {
                    foreach (Capture inherit in match.Groups["inherits"].Captures)
                    {
                        string inheritName = inherit.Value.Trim();

                        // if no name is given, then look to inherit a previous prop with the same name
                        if (inheritName.Length == 0)
                        {
                            inheritName = name;
                        }

                        inherits.Add(inheritName);
                    }
                }

                if (match.Groups["equals"].Success)
                {
                    hasEquals = true;
                }

                if (match.Groups["value"].Success)
                {
                    hasValue = true;
                    value = match.Groups["value"].Value.Trim();
                }

                // create the property
                if (hasEquals && hasValue)
                {
                    // fully-specified text property
                    parents.Peek().Add(new PropertyBag(name, value));

                    // ignore children
                }
                else if (hasEquals)
                {
                    // beginning of multi-line text property

                    // join all of the child lines together
                    foreach (IndentationTree textChild in child.Children)
                    {
                        // separate lines with a space
                        if (value != String.Empty)
                        {
                            value += " ";
                        }

                        value += textChild.Text;
                    }

                    parents.Peek().Add(new PropertyBag(name, value));
                }
                else
                {
                    // collection property

                    // look up the bases from this property's prior siblings
                    List<PropertyBag> bases = new List<PropertyBag>();

                    foreach (string baseName in inherits)
                    {
                        PropertyBag baseProp = null;

                        // walk up the concrete parent stack
                        foreach (PropertyBag parent in parents)
                        {
                            if (parent.Contains(baseName))
                            {
                                baseProp = parent[baseName];
                                break;
                            }
                        }

                        // if a concrete one wasn't found, look for an abstract one
                        if (baseProp == null)
                        {
                            foreach (PropertyBag abstractProp in abstractProps)
                            {
                                if (abstractProp.Contains(baseName))
                                {
                                    baseProp = abstractProp[baseName];
                                    break;
                                }
                            }
                        }

                        if (baseProp != null) bases.Add(baseProp);
                    }

                    PropertyBag prop = new PropertyBag(name, bases);

                    // add it to the appropriate prop
                    if (isAbstract)
                    {
                        abstractProps.Peek().Add(prop);
                    }
                    else
                    {
                        parents.Peek().Add(prop);
                    }

                    // recurse
                    parents.Push(prop);
                    ParseTree(child, parents, abstractProps);
                }
            }

            parents.Pop();
            abstractProps.Pop();
        }

        private static Regex sCommentRegex = new Regex(
            @"^(?<content>.*?)      # everything before the comment
               (?<comment>//.*)?$   # the comment",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static Regex sLineRegex = new Regex(
            @"
            ^(?<abstract>::)?             # may be abstract
             (?<name>..*?)                # name
             ((?<equals>=)(?<value>..*?)? # '= value' (value may be omitted)
             |(::(?<inherits>.*?))*       # or base props
             |                            # or nothing
             )$
            ",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static Regex sIncludeRegex = new Regex(
            @"
            ^\s*                # allow space before the include
             \#include          # the include command
             \s*                # allow space after the include
             ""(?<path>.*)""    # the include path
             $
            ",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
    }
}
