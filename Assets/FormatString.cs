using System;
using System.Runtime.CompilerServices;

public class FormatString {
    string value;

    public FormatString(string formattedString, params object[] arguments) {
        value = FormattableStringFactory.Create(formattedString, arguments).ToString();
    }

    override public string ToString() {
        return value;
    }
}
