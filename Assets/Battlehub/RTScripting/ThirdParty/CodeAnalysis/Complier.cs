using System.Collections.Generic;
using UnityEngine;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
using System;

namespace Battlehub.CodeAnalysis
{
    public interface ICompiler
    {
        void AddReference(byte[] image);
        void AddReference(string path);
        void ClearReferences();
        byte[] Compile(string[] text);
        
        [Obsolete("Use Compile(string[] text)")]
        byte[] Compile(string[] text, string[] references);
    }

    public class Complier : ICompiler
    {
        private readonly List<PortableExecutableReference> m_peReferences = new List<PortableExecutableReference>();

        public void AddReference(byte[] peImage)
        {
            m_peReferences.Add(MetadataReference.CreateFromImage(peImage));
        }

        public void AddReference(string path)
        {
            m_peReferences.Add(MetadataReference.CreateFromFile(path));
        }

        public void ClearReferences()
        {
            m_peReferences.Clear();
        }

        public byte[] Compile(string[] text)
        {
            PortableExecutableReference[] peReferences = m_peReferences.ToArray();
            return Compile(text, peReferences);
        }

        private static byte[] Compile(string[] text, PortableExecutableReference[] peReferences)
        {
            List<Microsoft.CodeAnalysis.SyntaxTree> syntaxTrees = new List<Microsoft.CodeAnalysis.SyntaxTree>();
            for (int i = 0; i < text.Length; ++i)
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(text[i]));
            }

            string assemblyName = Path.GetRandomFileName();
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: syntaxTrees,
                references: peReferences,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Debug.LogErrorFormat("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public byte[] Compile(string[] text, string[] extraReferences = null)
        {
            string assemblyName = Path.GetRandomFileName();
            HashSet<string> references = new HashSet<string>
            {
                typeof(object).Assembly.Location,
                typeof(Enumerable).Assembly.Location,
                typeof(UnityEngine.Object).Assembly.Location,
                typeof(Input).Assembly.Location,
                typeof(Rigidbody).Assembly.Location,
                typeof(AudioSource).Assembly.Location,
                typeof(Animator).Assembly.Location,
                typeof(ParticleSystem).Assembly.Location,
                typeof(Text).Assembly.Location,
                typeof(Canvas).Assembly.Location,
                Assembly.GetCallingAssembly().Location
            };

            if (extraReferences != null)
            {
                for (int i = 0; i < extraReferences.Length; ++i)
                {
                    if (!references.Contains(extraReferences[i]))
                    {
                        references.Add(extraReferences[i]);
                    }
                }
            }

            var refs = references.Select(r => MetadataReference.CreateFromFile(r)).ToArray();
            return Compile(text, refs);
        }
    }
}

