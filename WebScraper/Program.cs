using HtmlAgilityPack;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

string htmlContent = @"https://traditioninaction.org/HotTopics/f240_Dialogue_143.htm";

HtmlWeb web = new HtmlWeb()
{
    OverrideEncoding = Encoding.UTF8,
};

var htmlDoc = web.Load(htmlContent);

var topic = htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Id == "topicHeader" || node.Element("h3") != null);

var series = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='GreenSeries']");

var title = htmlDoc.DocumentNode.Descendants().FirstOrDefault(node => node.Name == "h1" || node.Name == "h4");

var author = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='author']");

var date = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='posted' or @id='sitation']");

var footNotes = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='footnotes']");
if (footNotes != null)
{
    var liNodes = footNotes.Descendants("li");

    foreach (var liNode in liNodes)
    {
        string liNodeText = liNode.InnerHtml;

        liNodeText.Replace("</li>", "").Replace("<li>", "");

        var matches = Regex.Split(liNodeText, "(<em>|</em>)");

        for (int i = 0; i < matches.Length; i++)
        {
            if (matches[i] == "<em>" || matches[i] == "</em>")
            {
                matches[i] = null;
            }
        }

    }
}



Console.WriteLine(topic.InnerText);
Console.WriteLine(series.InnerText.Trim());
Console.WriteLine(title.InnerText);
Console.WriteLine(author.InnerText);
Console.WriteLine(date.InnerText);
Console.WriteLine(footNotes.InnerHtml);

