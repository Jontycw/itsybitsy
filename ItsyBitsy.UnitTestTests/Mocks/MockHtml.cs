namespace ItsyBitsy.UnitTest
{
    public static class MockHtml
    {
        public static string Has7Links = @"<!doctype html>
            <html lang=""en"">
            <head>
              <meta charset=""utf-8"">
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

        public static string Has1Link = @"<!doctype html>
            <html lang=""en"">
            <head>
              <meta charset=""utf-8"">
              <title>The HTML5 Herald</title>
              <meta name=""description"" content=""The HTML5 Herald"">
              <meta name=""author"" content=""SitePoint"">
            </head>
            <body>
              <a href=""https://seed.co.za/FirstLink/"">first link</a>
            </body>
            </html>";

        public static string HasLink2 = $@"<!doctype html>
            <html lang=""en"">
            <head>
              <meta charset=""utf-8"">
              <title>The HTML5 Herald</title>
              <meta name=""description"" content=""The HTML5 Herald"">
              <meta name=""author"" content=""SitePoint"">
            </head>
            <body>
              <a href=""{Const.LINK2}"">first link</a>
            </body>
            </html>";
    }
}
