using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace CodeCracker.CSharp.Refactoring
{
    /// <summary>
    /// This analyzer produce 2 different hidden diagnostics one for regular string literals like
    /// "Hello" and another for verbatim string literalis like @"Hello".
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringRepresentationAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor RegularStringRule = new DiagnosticDescriptor(
            DiagnosticId.StringRepresentation_RegularString.ToDiagnosticId(),
            "Regular string",
            "Change to regular string",
            SupportedCategories.Refactoring,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.ForDiagnostic(DiagnosticId.StringRepresentation_RegularString));

        internal static DiagnosticDescriptor VerbatimStringRule = new DiagnosticDescriptor(
            DiagnosticId.StringRepresentation_VerbatimString.ToDiagnosticId(),
            "Verbatim string",
            "Change to verbatim string",
            SupportedCategories.Refactoring,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.ForDiagnostic(DiagnosticId.StringRepresentation_VerbatimString));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RegularStringRule, VerbatimStringRule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(Analyzer, SyntaxKind.StringLiteralExpression);

        private void Analyzer(SyntaxNodeAnalysisContext context)
        {
            var literalExpression = (LiteralExpressionSyntax)context.Node;
            var isVerbatim = literalExpression.Token.Text.Length > 0
                && literalExpression.Token.Text.StartsWith("@\"");

            context.ReportDiagnostic(
                Diagnostic.Create(
                    isVerbatim ? VerbatimStringRule : RegularStringRule,
                    literalExpression.GetLocation()));
        }
    }
}