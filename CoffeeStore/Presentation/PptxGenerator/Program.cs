using System.IO.Compression;
using System.Text;

string outputPath = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "CoffeeStore-Presentation.pptx");

string tempDir = Path.Combine(Path.GetTempPath(), "pptx_" + Guid.NewGuid().ToString("N"));

if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
Directory.CreateDirectory(tempDir);

string[] dirs = {
    "_rels", "docProps", "ppt", "ppt/_rels",
    "ppt/slides", "ppt/slides/_rels",
    "ppt/slideLayouts", "ppt/slideLayouts/_rels",
    "ppt/slideMasters", "ppt/slideMasters/_rels",
    "ppt/theme"
};

foreach (var d in dirs) Directory.CreateDirectory(Path.Combine(tempDir, d));

var utf8 = new UTF8Encoding(false);

File.WriteAllText(Path.Combine(tempDir, "[Content_Types].xml"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Types xmlns=""http://schemas.openxmlformats.org/package/2006/content-types"">
  <Default Extension=""rels"" ContentType=""application/vnd.openxmlformats-package.relationships+xml""/>
  <Default Extension=""xml"" ContentType=""application/xml""/>
  <Override PartName=""/ppt/presentation.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.presentation.main+xml""/>
  <Override PartName=""/ppt/slideMasters/slideMaster1.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slideMaster+xml""/>
  <Override PartName=""/ppt/slideLayouts/slideLayout1.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slideLayout+xml""/>
  <Override PartName=""/ppt/theme/theme1.xml"" ContentType=""application/vnd.openxmlformats-officedocument.theme+xml""/>
  <Override PartName=""/docProps/core.xml"" ContentType=""application/vnd.openxmlformats-package.core-properties+xml""/>
  <Override PartName=""/docProps/app.xml"" ContentType=""application/vnd.openxmlformats-officedocument.extended-properties+xml""/>
  <Override PartName=""/ppt/slides/slide1.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slide+xml""/>
  <Override PartName=""/ppt/slides/slide2.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slide+xml""/>
  <Override PartName=""/ppt/slides/slide3.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slide+xml""/>
  <Override PartName=""/ppt/slides/slide4.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slide+xml""/>
  <Override PartName=""/ppt/slides/slide5.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slide+xml""/>
  <Override PartName=""/ppt/slides/slide6.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slide+xml""/>
  <Override PartName=""/ppt/slides/slide7.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slide+xml""/>
  <Override PartName=""/ppt/slides/slide8.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slide+xml""/>
  <Override PartName=""/ppt/slides/slide9.xml"" ContentType=""application/vnd.openxmlformats-officedocument.presentationml.slide+xml""/>
</Types>", utf8);

File.WriteAllText(Path.Combine(tempDir, "_rels/.rels"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"" Target=""ppt/presentation.xml""/>
  <Relationship Id=""rId2"" Type=""http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties"" Target=""docProps/core.xml""/>
  <Relationship Id=""rId3"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties"" Target=""docProps/app.xml""/>
</Relationships>", utf8);

File.WriteAllText(Path.Combine(tempDir, "docProps/core.xml"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<cp:coreProperties xmlns:cp=""http://schemas.openxmlformats.org/package/2006/metadata/core-properties"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:dcterms=""http://purl.org/dc/terms/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <dc:title>CoffeeStore E-Commerce</dc:title>
  <dc:creator>Development Team</dc:creator>
  <cp:lastModifiedBy>Development Team</cp:lastModifiedBy>
  <dcterms:created xsi:type=""dcterms:W3CDTF"">2026-10-11T00:00:00Z</dcterms:created>
  <dcterms:modified xsi:type=""dcterms:W3CDTF"">2026-10-11T00:00:00Z</dcterms:modified>
</cp:coreProperties>", utf8);

File.WriteAllText(Path.Combine(tempDir, "docProps/app.xml"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Properties xmlns=""http://schemas.openxmlformats.org/officeDocument/2006/extended-properties"" xmlns:vt=""http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes"">
  <Application>CoffeeStore</Application>
</Properties>", utf8);

File.WriteAllText(Path.Combine(tempDir, "ppt/presentation.xml"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<p:presentation xmlns:a=""http://schemas.openxmlformats.org/drawingml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:p=""http://schemas.openxmlformats.org/presentationml/2006/main"">
  <p:sldMasterIdLst><p:sldMasterId id=""2147483648"" r:id=""rId1""/></p:sldMasterIdLst>
  <p:sldIdLst>
    <p:sldId id=""256"" r:id=""rId2""/>
    <p:sldId id=""257"" r:id=""rId3""/>
    <p:sldId id=""258"" r:id=""rId4""/>
    <p:sldId id=""259"" r:id=""rId5""/>
    <p:sldId id=""260"" r:id=""rId6""/>
    <p:sldId id=""261"" r:id=""rId7""/>
    <p:sldId id=""262"" r:id=""rId8""/>
    <p:sldId id=""263"" r:id=""rId9""/>
    <p:sldId id=""264"" r:id=""rId10""/>
  </p:sldIdLst>
  <p:sldSz cx=""12192000"" cy=""6858000""/>
  <p:notesSz cx=""6858000"" cy=""9144000""/>
</p:presentation>", utf8);

File.WriteAllText(Path.Combine(tempDir, "ppt/_rels/presentation.xml.rels"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster"" Target=""slideMasters/slideMaster1.xml""/>
  <Relationship Id=""rId2"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme"" Target=""theme/theme1.xml""/>
  <Relationship Id=""rId3"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"" Target=""slides/slide1.xml""/>
  <Relationship Id=""rId4"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"" Target=""slides/slide2.xml""/>
  <Relationship Id=""rId5"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"" Target=""slides/slide3.xml""/>
  <Relationship Id=""rId6"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"" Target=""slides/slide4.xml""/>
  <Relationship Id=""rId7"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"" Target=""slides/slide5.xml""/>
  <Relationship Id=""rId8"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"" Target=""slides/slide6.xml""/>
  <Relationship Id=""rId9"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"" Target=""slides/slide7.xml""/>
  <Relationship Id=""rId10"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"" Target=""slides/slide8.xml""/>
</Relationships>", utf8);

File.WriteAllText(Path.Combine(tempDir, "ppt/theme/theme1.xml"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<a:theme xmlns:a=""http://schemas.openxmlformats.org/drawingml/2006/main"" name=""CoffeeStore Dark"">
  <a:themeElements>
    <a:clrScheme name=""CoffeeStore"">
      <a:dk1><a:srgbClr val=""0A0A0A""/></a:dk1>
      <a:lt1><a:srgbClr val=""FFFFFF""/></a:lt1>
      <a:dk2><a:srgbClr val=""1A1A1A""/></a:dk2>
      <a:lt2><a:srgbClr val=""F2F2F2""/></a:lt2>
      <a:accent1><a:srgbClr val=""C0392B""/></a:accent1>
      <a:accent2><a:srgbClr val=""2980B9""/></a:accent2>
      <a:accent3><a:srgbClr val=""27AE60""/></a:accent3>
      <a:accent4><a:srgbClr val=""8E44AD""/></a:accent4>
      <a:accent5><a:srgbClr val=""F39C12""/></a:accent5>
      <a:accent6><a:srgbClr val=""16A085""/></a:accent6>
      <a:hlink><a:srgbClr val=""2980B9""/></a:hlink>
      <a:folHlink><a:srgbClr val=""8E44AD""/></a:folHlink>
    </a:clrScheme>
    <a:fontScheme name=""CoffeeStore"">
      <a:majorFont><a:latin typeface=""Segoe UI""/><a:ea typeface=""/><a:cs typeface=""/></a:majorFont>
      <a:minorFont><a:latin typeface=""Segoe UI""/><a:ea typeface=""/><a:cs typeface=""/></a:minorFont>
    </a:fontScheme>
    <a:fmtScheme name=""CoffeeStore"">
      <a:fillStyleLst>
        <a:solidFill><a:schemeClr val=""phClr""/></a:solidFill>
        <a:solidFill><a:schemeClr val=""phClr""/></a:solidFill>
        <a:solidFill><a:schemeClr val=""phClr""/></a:solidFill>
      </a:fillStyleLst>
      <a:lnStyleLst>
        <a:ln w=""6350""><a:solidFill><a:schemeClr val=""phClr""/></a:solidFill></a:ln>
        <a:ln w=""12700""><a:solidFill><a:schemeClr val=""phClr""/></a:solidFill></a:ln>
        <a:ln w=""19050""><a:solidFill><a:schemeClr val=""phClr""/></a:solidFill></a:ln>
      </a:lnStyleLst>
      <a:effectStyleLst>
        <a:effectStyle><a:effectLst/></a:effectStyle>
        <a:effectStyle><a:effectLst/></a:effectStyle>
        <a:effectStyle><a:effectLst/></a:effectStyle>
      </a:effectStyleLst>
      <a:bgFillStyleLst>
        <a:solidFill><a:schemeClr val=""phClr""/></a:solidFill>
        <a:solidFill><a:schemeClr val=""phClr""/></a:solidFill>
        <a:solidFill><a:schemeClr val=""phClr""/></a:solidFill>
      </a:bgFillStyleLst>
    </a:fmtScheme>
  </a:themeElements>
</a:theme>", utf8);

File.WriteAllText(Path.Combine(tempDir, "ppt/slideMasters/slideMaster1.xml"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<p:sldMaster xmlns:a=""http://schemas.openxmlformats.org/drawingml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:p=""http://schemas.openxmlformats.org/presentationml/2006/main"">
  <p:cSld>
    <p:bg><p:bgPr><a:solidFill><a:srgbClr val=""0A0A0A""/></a:solidFill></p:bgPr></p:bg>
    <p:spTree>
      <p:nvGrpSpPr><p:cNvPr id=""1"" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr>
      <p:grpSpPr><a:xfrm><a:off x=""0"" y=""0""/><a:ext cx=""0"" cy=""0""/><a:chOff x=""0"" y=""0""/><a:chExt cx=""0"" cy=""0""/></a:xfrm></p:grpSpPr>
    </p:spTree>
  </p:cSld>
  <p:clrMap bg1=""lt1"" tx1=""dk1"" bg2=""lt2"" tx2=""dk2"" accent1=""accent1"" accent2=""accent2"" accent3=""accent3"" accent4=""accent4"" accent5=""accent5"" accent6=""accent6"" hlink=""hlink"" folHlink=""folHlink""/>
  <p:sldLayoutIdLst><p:sldLayoutId id=""2147483649"" r:id=""rId1""/></p:sldLayoutIdLst>
  <p:txStyles>
    <p:titleStyle><a:lvl1pPr><a:defRPr sz=""4400"" b=""1""><a:solidFill><a:srgbClr val=""FFFFFF""/></a:solidFill><a:latin typeface=""Segoe UI""/></a:defRPr></a:lvl1pPr></p:titleStyle>
    <p:bodyStyle><a:lvl1pPr><a:defRPr sz=""2400""><a:solidFill><a:srgbClr val=""C0392B""/></a:solidFill><a:latin typeface=""Segoe UI""/></a:defRPr></a:lvl1pPr></p:bodyStyle>
    <p:otherStyle><a:lvl1pPr><a:defRPr sz=""1800""><a:solidFill><a:srgbClr val=""CCCCCC""/></a:solidFill><a:latin typeface=""Segoe UI""/></a:defRPr></a:lvl1pPr></p:otherStyle>
  </p:txStyles>
</p:sldMaster>", utf8);

File.WriteAllText(Path.Combine(tempDir, "ppt/slideMasters/_rels/slideMaster1.xml.rels"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout"" Target=""../slideLayouts/slideLayout1.xml""/>
  <Relationship Id=""rId2"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme"" Target=""../theme/theme1.xml""/>
</Relationships>", utf8);

File.WriteAllText(Path.Combine(tempDir, "ppt/slideLayouts/slideLayout1.xml"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<p:sldLayout xmlns:a=""http://schemas.openxmlformats.org/drawingml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:p=""http://schemas.openxmlformats.org/presentationml/2006/main"" type=""blank"" preserve=""1"">
  <p:cSld name=""Blank"">
    <p:spTree>
      <p:nvGrpSpPr><p:cNvPr id=""1"" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr>
      <p:grpSpPr><a:xfrm><a:off x=""0"" y=""0""/><a:ext cx=""0"" cy=""0""/><a:chOff x=""0"" y=""0""/><a:chExt cx=""0"" cy=""0""/></a:xfrm></p:grpSpPr>
    </p:spTree>
  </p:cSld>
  <p:clrMapOvr><a:overrideClrMapping bg1=""lt1"" tx1=""dk1"" bg2=""lt2"" tx2=""dk2"" accent1=""accent1"" accent2=""accent2"" accent3=""accent3"" accent4=""accent4"" accent5=""accent5"" accent6=""accent6"" hlink=""hlink"" folHlink=""folHlink""/></p:clrMapOvr>
</p:sldLayout>", utf8);

File.WriteAllText(Path.Combine(tempDir, "ppt/slideLayouts/_rels/slideLayout1.xml.rels"),
    @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster"" Target=""../slideMasters/slideMaster1.xml""/>
</Relationships>", utf8);

var slides = new (string Title, string Subtitle, string[] Bullets)[]
{
    ("CoffeeStore E-Commerce", "\u0646\u0638\u0627\u0645 \u0625\u062F\u0627\u0631\u0629 \u0645\u062A\u062C\u0631 \u0642\u0647\u0648\u0629 \u0625\u0644\u0643\u062A\u0631\u0648\u0646\u064A", new[]
    {
        "\u0645\u0646\u0635\u0629 \u062A\u062C\u0627\u0631\u0629 \u0625\u0644\u0643\u062A\u0631\u0648\u0646\u064A\u0629 \u0645\u062A\u0643\u0627\u0645\u0644\u0629 \u0645\u0628\u0646\u064A\u0629 \u0628\u0623\u062D\u062F\u062B \u0645\u0639\u0645\u0627\u0631\u0627\u062A .NET",
        "\u0645\u0639\u0645\u0627\u0631\u064A\u0629 \u0646\u0638\u064A\u0641\u0629 - \u0623\u062F\u0627\u0621 \u0639\u0627\u0644\u064A - \u0642\u0627\u0628\u0644\u064A\u0629 \u062A\u0648\u0633\u0639 \u063A\u064A\u0631 \u0645\u062D\u062F\u0648\u062F",
        ".NET 9 - Clean Architecture - CQRS - Unit of Work - Specification Pattern"
    }),
    ("\u0644\u064A\u0647 \u0645\u0639\u0645\u0627\u0631\u064A\u0629 \u062F\u064A\u061F", "\u0645\u0634\u0627\u0643\u0644 \u0627\u0644\u0644\u064A \u0628\u0646\u062D\u0644\u0647\u0627", new[]
    {
        "\u0627\u0644\u0643\u0648\u062F \u0645\u062A\u0634\u0627\u0628\u0643 \u0648\u0635\u0639\u0628 \u0627\u0644\u062A\u0639\u062F\u064A\u0644 \u2190 Clean Architecture",
        "\u0635\u0639\u0648\u0628\u0629 \u0627\u062E\u062A\u0628\u0627\u0631 \u0627\u0644\u0643\u0648\u062F \u2190 Dependency Injection + Repository Pattern",
        "\u0627\u0633\u062A\u0639\u0644\u0627\u0645\u0627\u062A SQL \u0645\u0643\u0631\u0631\u0629 \u2190 Specification Pattern",
        "\u0645\u0634\u0627\u0643\u0644 \u0641\u064A \u0627\u0644\u0645\u0639\u0627\u0645\u0644\u0627\u062A \u0627\u0644\u0645\u0639\u0642\u062F\u0629 \u2190 Unit of Work Pattern",
        "\u0635\u0639\u0648\u0628\u0629 \u0625\u0636\u0627\u0641\u0629 \u0645\u064A\u0632\u0627\u062A \u062C\u062F\u064A\u062F\u0629 \u2190 Result Pattern + DDD"
    }),
    ("\u0647\u064A\u0643\u0644 \u0627\u0644\u0645\u0634\u0631\u0648\u0639", "4 \u0637\u0628\u0642\u0627\u062A \u0646\u0638\u064A\u0641\u0629 \u0648\u0645\u0646\u0641\u0635\u0644\u0629", new[]
    {
        "ECommerce.Api \u2192 Controllers + Middleware + JWT Auth + Swagger",
        "ECommerce.Application \u2192 Services + DTOs + Business Logic + Validation",
        "ECommerce.Domain \u2192 Entities + Interfaces + Domain Rules + Events",
        "ECommerce.Infrastructure \u2192 EF Core + Redis + Repositories + Identity",
        "\u0643\u0644 \u0637\u0628\u0642\u0629 \u0628\u062A\u0639\u0631\u0641 \u0639\u0644\u0649 \u0627\u0644\u0644\u064A \u062A\u062D\u062A\u0647\u0627 \u0628\u0633 - \u0627\u0646\u0641\u0635\u0627\u0644 \u062A\u0627\u0645"
    }),
    ("Design Patterns \u0627\u0644\u0645\u0633\u062A\u062E\u062F\u0645\u0629", "\u0627\u062E\u062A\u064A\u0627\u0631\u0627\u062A \u0645\u062F\u0631\u0648\u0633\u0629 \u0644\u0643\u0644 \u0646\u0645\u0637", new[]
    {
        "Repository Pattern \u2192 \u064A\u0641\u0635\u0644 \u0637\u0628\u0642\u0629 \u0627\u0644\u0628\u064A\u0627\u0646\u0627\u062A \u0639\u0646 \u0627\u0644\u0628\u0632\u0646\u0633 \u0644\u0648\u062C\u064A\u0643",
        "Unit of Work \u2192 \u0628\u064A\u0636\u0645\u0646 \u0625\u0646 \u0643\u0644 \u0627\u0644\u0639\u0645\u0644\u064A\u0627\u062A \u062A\u062A\u0645 \u0641\u064A \u0645\u0639\u0627\u0645\u0644\u0629 \u0648\u0627\u062D\u062F\u0629",
        "Specification Pattern \u2192 \u0627\u0644\u0640 queries \u0628\u0642\u0627\u0628\u0644\u0629 \u0644\u0625\u0639\u0627\u062F\u0629 \u0627\u0644\u0627\u0633\u062A\u062E\u062F\u0627\u0645 \u0648\u0627\u0644\u062A\u0631\u0643\u064A\u0628",
        "Result Pattern \u2192 \u0628\u062F\u0644 \u0631\u0645\u064A Exceptions \u0628\u0646\u0631\u062C\u0639 Result \u0648\u0627\u0636\u062D",
        "Dependency Injection \u2192 \u0643\u0644 \u062E\u062F\u0645\u0629 \u0628\u062A\u0627\u062E\u062F \u0627\u0639\u062A\u0645\u0627\u062F\u064A\u0627\u062A\u0647\u0627 \u0645\u0646 \u0628\u0631\u0647"
    }),
    ("\u0645\u0639\u0627\u0644\u062C\u0629 \u0627\u0644\u0623\u062E\u0637\u0627\u0621", "Result Pattern \u0628\u0627\u0644\u062A\u0641\u0635\u064A\u0644", new[]
    {
        "\u0628\u062F\u0644 try/catch \u0648\u0631\u0645\u064A Exceptions \u0641\u064A \u0643\u0644 \u0645\u0643\u0627\u0646",
        "\u0628\u0646\u0631\u062C\u0639 Result<T> \u0641\u064A\u0647 Value + Errors + IsSuccess",
        "\u0643\u0644 Error \u0644\u0647 Code + Description + Type",
        "ErrorType: NotFound, Validation, Conflict, Unauthorized",
        "\u0643\u0648\u062F \u0646\u0638\u064A\u0641 \u0628\u062F\u0648\u0646 \u0645\u0639\u0627\u0644\u062C\u0629 \u0623\u062E\u0637\u0627\u0621 \u0645\u062A\u0634\u0627\u0628\u0643\u0629"
    }),
    ("\u0646\u0638\u0627\u0645 \u0627\u0644\u0637\u0644\u0628\u0627\u062A", "Order Creation Flow", new[]
    {
        "1. \u0627\u0644\u062A\u062D\u0642\u0642 \u0645\u0646 \u0627\u0644\u0633\u0644\u0629 Basket \u0645\u0646 Redis",
        "2. \u0627\u0644\u062A\u062D\u0642\u0642 \u0645\u0646 \u0637\u0631\u064A\u0642\u0629 \u0627\u0644\u062A\u0648\u0635\u064A\u0644 \u0645\u0648\u062C\u0648\u062F\u0629 \u0648\u0645\u062A\u0627\u062D\u0629",
        "3. \u0627\u0644\u062A\u062D\u0642\u0642 \u0645\u0646 \u0627\u0644\u0639\u0645\u064A\u0644 \u0628\u0627\u0644\u0628\u062D\u062B \u0628\u0627\u0644\u0625\u064A\u0645\u064A\u0644",
        "4. \u0625\u0646\u0634\u0627\u0621 OrderItems \u0645\u0646 \u0643\u0644 \u0645\u0646\u062A\u062C \u0641\u064A \u0627\u0644\u0633\u0644\u0629",
        "5. \u062D\u0641\u0638 \u0627\u0644\u0637\u0644\u0628 \u0628\u0640 Unit of Work \u0641\u064A \u0645\u0639\u0627\u0645\u0644\u0629 \u0648\u0627\u062D\u062F\u0629"
    }),
    ("\u0627\u0644\u0641\u0648\u0627\u0626\u062F \u0644\u0644\u0645\u0633\u062A\u062E\u062F\u0645\u064A\u0646", "\u0627\u064A\u0647 \u0627\u0644\u0644\u064A \u0628\u064A\u0643\u0633\u0628\u0647 \u0627\u0644\u0645\u0633\u062A\u062E\u062F\u0645", new[]
    {
        "\u0633\u0631\u0639\u0629 \u0641\u0627\u0626\u0642\u0629 \u2192 Redis Caching \u0644\u0644\u0628\u064A\u0627\u0646\u0627\u062A \u0627\u0644\u0645\u062A\u0643\u0631\u0631\u0629",
        "\u0623\u0645\u0627\u0646 \u0643\u0627\u0645\u0644 \u2192 JWT + Role-based Authorization",
        "\u062A\u062C\u0631\u0628\u0629 \u0633\u0644\u0633\u0629 \u2192 \u0633\u0644\u0629 \u0630\u0643\u064A\u0629 \u0645\u0633\u062A\u0645\u0631\u0629 \u0639\u0644\u0649 \u0627\u0644\u0633\u064A\u0631\u0641\u0631",
        "\u0645\u062A\u0648\u0627\u0641\u0642 \u0645\u0639 \u0643\u0644 \u0627\u0644\u0623\u062C\u0647\u0632\u0629 \u2192 API + CORS \u062C\u0627\u0647\u0632 \u0644\u0623\u064A Frontend"
    }),
    ("\u0627\u0644\u0645\u0634\u0631\u0648\u0639 \u062C\u0627\u0647\u0632 \u0644\u0644\u062A\u0648\u0633\u0639", "\u062E\u0637\u0648\u0627\u062A \u0642\u0627\u062F\u0645\u0629", new[]
    {
        "\u062C\u0627\u0647\u0632: \u0633\u0644\u0629 + \u0637\u0644\u0628\u0627\u062A + \u0645\u0635\u0627\u062F\u0642\u0629 + API",
        "\u0627\u0644\u0642\u0627\u062F\u0645: \u062F\u0641\u0639 + \u062A\u062A\u0628\u0639 \u0634\u062D\u0646\u0627\u062A + \u0625\u0634\u0639\u0627\u0631\u0627\u062A",
        "\u0645\u062A\u062C\u0631 \u0642\u0647\u0648\u0629 \u0625\u0644\u0643\u062A\u0631\u0648\u0646\u064A \u0643\u0627\u0645\u0644 \u0645\u0646 \u0627\u0644\u0635\u0641\u0631",
        "CoffeeStore - Built with Clean Architecture"
    })
};

for (int i = 0; i < slides.Length; i++)
{
    var (title, subtitle, bullets) = slides[i];
    int slideNum = i + 1;

    string bulletsXml = "";
    foreach (var bullet in bullets)
    {
        string escaped = bullet.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        bulletsXml += $@"          <a:p><a:pPr rtl=""1"" algn=""l"" marL=""285750"" indent=""-285750""><a:buChar char=""&#8226;""/></a:pPr><a:r><a:rPr lang=""ar-SA"" sz=""1800""><a:solidFill><a:srgbClr val=""CCCCCC""/></a:solidFill><a:latin typeface=""Segoe UI""/></a:rPr><a:t>{escaped}</a:t></a:r></a:p>\n";
    }

    string escapedTitle = title.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    string escapedSubtitle = subtitle.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    string slideXml = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<p:sld xmlns:a=""http://schemas.openxmlformats.org/drawingml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:p=""http://schemas.openxmlformats.org/presentationml/2006/main"">
  <p:cSld>
    <p:bg><p:bgPr><a:solidFill><a:srgbClr val=""0A0A0A""/></a:solidFill></p:bgPr></p:bg>
    <p:spTree>
      <p:nvGrpSpPr><p:cNvPr id=""1"" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr>
      <p:grpSpPr><a:xfrm><a:off x=""0"" y=""0""/><a:ext cx=""0"" cy=""0""/><a:chOff x=""0"" y=""0""/><a:chExt cx=""0"" cy=""0""/></a:xfrm></p:grpSpPr>
      <p:sp>
        <p:nvSpPr><p:cNvPr id=""2"" name=""Title""/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
        <p:spPr/>
        <p:txBody>
          <a:bodyPr rtlCol=""1"" anchor=""t""/><a:lstStyle/>
          <a:p><a:pPr rtl=""1"" algn=""l""><a:lnSpc><a:spcPct val=""150000""/></a:lnSpc></a:pPr><a:r><a:rPr lang=""ar-SA"" sz=""4400"" b=""1""><a:solidFill><a:srgbClr val=""FFFFFF""/></a:solidFill><a:latin typeface=""Segoe UI""/></a:rPr><a:t>{escapedTitle}</a:t></a:r></a:p>
          <a:p><a:pPr rtl=""1"" algn=""l""/><a:r><a:rPr lang=""ar-SA"" sz=""2400""><a:solidFill><a:srgbClr val=""C0392B""/></a:solidFill><a:latin typeface=""Segoe UI""/></a:rPr><a:t>{escapedSubtitle}</a:t></a:r></a:p>
        </p:txBody>
      </p:sp>
      <p:sp>
        <p:nvSpPr><p:cNvPr id=""3"" name=""Content""/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
        <p:spPr/>
        <p:txBody>
          <a:bodyPr rtlCol=""1"" anchor=""t""/><a:lstStyle/>
{bulletsXml}        </p:txBody>
      </p:sp>
    </p:spTree>
  </p:cSld>
  <p:clrMapOvr><a:overrideClrMapping bg1=""lt1"" tx1=""dk1"" bg2=""lt2"" tx2=""dk2"" accent1=""accent1"" accent2=""accent2"" accent3=""accent3"" accent4=""accent4"" accent5=""accent5"" accent6=""accent6"" hlink=""hlink"" folHlink=""folHlink""/></p:clrMapOvr>
</p:sld>";

    string slidePath = Path.Combine(tempDir, $"ppt/slides/slide{slideNum}.xml");
    File.WriteAllText(slidePath, slideXml, utf8);

    string relsXml = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout"" Target=""../slideLayouts/slideLayout1.xml""/>
</Relationships>";

    File.WriteAllText(Path.Combine(tempDir, $"ppt/slides/_rels/slide{slideNum}.xml.rels"), relsXml, utf8);
}

if (File.Exists(outputPath)) File.Delete(outputPath);
ZipFile.CreateFromDirectory(tempDir, outputPath);
Directory.Delete(tempDir, true);

Console.WriteLine($"Presentation saved to: {outputPath}");
