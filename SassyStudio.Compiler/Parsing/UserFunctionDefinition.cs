﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class UserFunctionDefinition : ComplexItem
    {
        private readonly List<FunctionArgumentDefinition> _Arguments = new List<FunctionArgumentDefinition>(1);
        public UserFunctionDefinition()
        {
            _Arguments = new List<FunctionArgumentDefinition>(1);
        }

        public AtRule Rule { get; protected set; }
        public TokenItem Name { get; protected set; }
        public TokenItem OpenBrace { get; protected set; }
        public IList<FunctionArgumentDefinition> Arguments { get { return _Arguments; } }
        public TokenItem CloseBrace { get; protected set; }
        public UserFunctionBody Body { get; protected set; }
        public string FunctionName { get; set; }

        public override bool IsValid { get { return Name != null && Name.Length > 0; } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "function"))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                if (stream.Current.Type == TokenType.Function)
                {
                    Name = Children.AddCurrentAndAdvance(stream, SassClassifierType.UserFunctionDefinition);
                    FunctionName = text.GetText(Name.Start, Name.Length);
                }

                if (stream.Current.Type == TokenType.OpenFunctionBrace)
                    Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);

                while (!IsArgumentTerminator(stream.Current.Type))
                {
                    var argument = itemFactory.CreateSpecific<FunctionArgumentDefinition>(this, text, stream);
                    if (argument == null || !argument.Parse(itemFactory, text, stream))
                        break;

                    Arguments.Add(argument);
                    Children.Add(argument);
                }

                if (stream.Current.Type == TokenType.CloseFunctionBrace)
                    Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);

                var body = new UserFunctionBody();
                if (body.Parse(itemFactory, text, stream))
                {
                    Body = body;
                    Children.Add(body);
                }
            }

            return Children.Count > 0;
        }

        public override void Freeze()
        {
            base.Freeze();
            _Arguments.TrimExcess();
        }

        public string GetName(ITextProvider text)
        {
            if (IsValid)
                return text.GetText(Name.Start, Name.Length);

            return null;
        }

        //public override IEnumerable<VariableName> GetDefinedVariables(int position)
        //{
        //    var variables = base.GetDefinedVariables(position);

        //    // only include defined arguments if position is in the body
        //    if (Body != null && position > Body.Start)
        //        variables = variables.Concat(Arguments.Select(x => x.Variable).Where(x => x.IsValid).Select(x => x.Name));

        //    return variables;
        //}

        static bool IsArgumentTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.CloseFunctionBrace:
                case TokenType.Comma:
                case TokenType.OpenCurlyBrace:
                    return true;
            }

            return false;
        }
    }
}
