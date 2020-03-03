using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using RestSharp;

namespace Tests.Unit.Helpers
{
    public static class QueryTester
    {
        public static void CheckQueryParameter(this Parameter[] parameters, int index, string name, string value, ParameterType type)
        {
            parameters[index].Name.Should().Be(name);
            parameters[index].Type.Should().Be(type);
            parameters[index].Value.Should().Be(value);
        }
    }
}
