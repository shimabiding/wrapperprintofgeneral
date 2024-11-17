using System;
using System.Windows.Automation;

namespace moduleAUI {

class Program {
    static void Main(string[] args) {
        try {
            AutomationElement tw = FindWindowByName("GeneralPrintWrapper");
            Console.WriteLine(tw.GetCurrentPropertyValue(AutomationElement.ClassNameProperty));
            if(tw == null) return;
            AutomationElement te = FindElementByAutomationId(tw, "textBox1");
            Console.WriteLine(te.GetCurrentPropertyValue(AutomationElement.ClassNameProperty));
            if(te == null) return;
            SetTextToElement(te, "09d9aAD");
        }
        catch(Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }

    static AutomationElement FindWindowByName(string name) {
        return AutomationElement.RootElement.FindFirst(
            TreeScope.Children,
            new PropertyCondition(AutomationElement.NameProperty, name));
    }

    static AutomationElement FindElementByAutomationId(AutomationElement parent, string id) {
        return parent.FindFirst(
            TreeScope.Descendants,
            new PropertyCondition(AutomationElement.AutomationIdProperty, id));
    }

    static void SetTextToElement(AutomationElement element, string text) {
        try {
            var valuePattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            if (valuePattern == null) return;
            valuePattern.SetValue(text);
        }
        catch (InvalidOperationException ex) {
            Console.WriteLine(ex.Message);
        }
    }
}

}