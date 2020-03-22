using System;
using System.Collections.Generic;
using System.Text;

namespace ItsyBitsy.UnitTest
{
    public static class MockHtml
    {
        public static string Has5Links = @"<!doctype html>
            <html lang=""en"">
            <head>
              <meta charset=""utf-8"">
              <meta property=""og:url"" content=""https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test"">
              <title>The HTML5 Herald</title>
              <meta name=""description"" content=""The HTML5 Herald"">
              <meta name=""author"" content=""SitePoint"">
              <link rel=""stylesheet"" href=""css/styles.css?v=1.0"">
            </head>
            <body>
              <a href=""a"">first link</a>
              <a href=""b"">second link</a>
              <script src=""js/scripts.js""></script>
              <a href=""c/"">third link</a>
              <div>word</div>
              <a href=""/d"">fourth link</a>
              <a href=""A"">fith link</a>
            </body>
            </html>";
    }
}
