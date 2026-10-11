using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

string outputPath = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "CoffeeStore-Presentation.pptx");

var slideData = new[]
{
    new SlideData
    {
        Title = "CoffeeStore E-Commerce",
        Subtitle = "\u0646\u0638\u0627\u0645 \u0625\u062F\u0627\u0631\u0629 \u0645\u062A\u062C\u0631 \u0642\u0647\u0648\u0629 \u0625\u0644\u0643\u062A\u0631\u0648\u0646\u064A",
        BulletPoints = new[]
        {
            "\u0645\u0646\u0635\u0629 \u062A\u062C\u0627\u0631\u0629 \u0625\u0644\u0643\u062A\u0631\u0648\u0646\u064A\u0629 \u0645\u062A\u0643\u0627\u0645\u0644\u0629 \u0645\u0628\u0646\u064A\u0629 \u0628\u0623\u062D\u062F\u062B \u0645\u0639\u0645\u0627\u0631\u0627\u062A .NET",
            "\u0645\u0639\u0645\u0627\u0631\u064A\u0629 \u0646\u0638\u064A\u0641\u0629 - \u0623\u062F\u0627\u0621 \u0639\u0627\u0644\u064A - \u0642\u0627\u0628\u0644\u064A\u0629 \u062A\u0648\u0633\u0639 \u063A\u064A\u0631 \u0645\u062D\u062F\u0648\u062F",
            ".NET 9 - Clean Architecture - CQRS - Unit of Work - Specification Pattern"
        }
    },
    new SlideData
    {
        Title = "\u0644\u064A\u0647 \u0645\u0639\u0645\u0627\u0631\u064A\u0629 \u062F\u064A\u061F",
        Subtitle = "\u0645\u0634\u0627\u0643\u0644 \u0627\u0644\u0644\u064A \u0628\u0646\u062D\u0644\u0647\u0627",
        BulletPoints = new[]
        {
            "\u0627\u0644\u0643\u0648\u062F \u0645\u062A\u0634\u0627\u0628\u0643 \u0648\u0635\u0639\u0628 \u0627\u0644\u062A\u0639\u062F\u064A\u0644 \u2190 Clean Architecture",
            "\u0635\u0639\u0648\u0628\u0629 \u0627\u062E\u062A\u0628\u0627\u0631 \u0627\u0644\u0643\u0648\u062F \u2190 Dependency Injection + Repository Pattern",
            "\u0627\u0633\u062A\u0639\u0644\u0627\u0645\u0627\u062A SQL \u0645\u0643\u0631\u0631\u0629 \u2190 Specification Pattern",
            "\u0645\u0634\u0627\u0643\u0644 \u0641\u064A \u0627\u0644\u0645\u0639\u0627\u0645\u0644\u0627\u062A \u0627\u0644\u0645\u0639\u0642\u062F\u0629 \u2190 Unit of Work Pattern",
            "\u0635\u0639\u0648\u0628\u0629 \u0625\u0636\u0627\u0641\u0629 \u0645\u064A\u0632\u0627\u062A \u062C\u062F\u064A\u062F\u0629 \u2190 Result Pattern + DDD"
        }
    },
    new SlideData
    {
        Title = "\u0647\u064A\u0643\u0644 \u0627\u0644\u0645\u0634\u0631\u0648\u0639",
        Subtitle = "4 \u0637\u0628\u0642\u0627\u062A \u0646\u0638\u064A\u0641\u0629 \u0648\u0645\u0646\u0641\u0635\u0644\u0629",
        BulletPoints = new[]
        {
            "ECommerce.Api \u2192 Controllers + Middleware + JWT Auth + Swagger",
            "ECommerce.Application \u2192 Services + DTOs + Business Logic + Validation",
            "ECommerce.Domain \u2192 Entities + Interfaces + Domain Rules + Events",
            "ECommerce.Infrastructure \u2192 EF Core + Redis + Repositories + Identity",
            "\u0643\u0644 \u0637\u0628\u0642\u0629 \u0628\u062A\u0639\u0631\u0641 \u0639\u0644\u0649 \u0627\u0644\u0644\u064A \u062A\u062D\u062A\u0647\u0627 \u0628\u0633 - \u0627\u0646\u0641\u0635\u0627\u0644 \u062A\u0627\u0645"
        }
    },
    new SlideData
    {
        Title = "Design Patterns \u0627\u0644\u0645\u0633\u062A\u062E\u062F\u0645\u0629",
        Subtitle = "\u0627\u062E\u062A\u064A\u0627\u0631\u0627\u062A \u0645\u062F\u0631\u0648\u0633\u0629 \u0644\u0643\u0644 \u0646\u0645\u0637",
        BulletPoints = new[]
        {
            "Repository Pattern \u2192 \u064A\u0641\u0635\u0644 \u0637\u0628\u0642\u0629 \u0627\u0644\u0628\u064A\u0627\u0646\u0627\u062A \u0639\u0646 \u0627\u0644\u0628\u0632\u0646\u0633 \u0644\u0648\u062C\u064A\u0643",
            "Unit of Work \u2192 \u0628\u064A\u0636\u0645\u0646 \u0625\u0646 \u0643\u0644 \u0627\u0644\u0639\u0645\u0644\u064A\u0627\u062A \u062A\u062A\u0645 \u0641\u064A \u0645\u0639\u0627\u0645\u0644\u0629 \u0648\u0627\u062D\u062F\u0629",
            "Specification Pattern \u2192 \u0627\u0644\u0640 queries \u0628\u0642\u0627\u0628\u0644\u0629 \u0644\u0625\u0639\u0627\u062F\u0629 \u0627\u0644\u0627\u0633\u062A\u062E\u062F\u0627\u0645 \u0648\u0627\u0644\u062A\u0631\u0643\u064A\u0628",
            "Result Pattern \u2192 \u0628\u062F\u0644 \u0631\u0645\u064A Exceptions \u0628\u0646\u0631\u062C\u0639 Result \u0648\u0627\u0636\u062D",
            "Dependency Injection \u2192 \u0643\u0644 \u062E\u062F\u0645\u0629 \u0628\u062A\u0627\u062E\u062F \u0627\u0639\u062A\u0645\u0627\u062F\u064A\u0627\u062A\u0647\u0627 \u0645\u0646 \u0628\u0631\u0647"
        }
    },
    new SlideData
    {
        Title = "\u0645\u0639\u0627\u0644\u062C\u0629 \u0627\u0644\u0623\u062E\u0637\u0627\u0621",
        Subtitle = "Result Pattern \u0628\u0627\u0644\u062A\u0641\u0635\u064A\u0644",
        BulletPoints = new[]
        {
            "\u0628\u062F\u0644 try/catch \u0648\u0631\u0645\u064A Exceptions \u0641\u064A \u0643\u0644 \u0645\u0643\u0627\u0646",
            "\u0628\u0646\u0631\u062C\u0639 Result<T> \u0641\u064A\u0647 Value + Errors + IsSuccess",
            "\u0643\u0644 Error \u0644\u0647 Code + Description + Type",
            "ErrorType: NotFound, Validation, Conflict, Unauthorized",
            "\u0643\u0648\u062F \u0646\u0638\u064A\u0641 \u0628\u062F\u0648\u0646 \u0645\u0639\u0627\u0644\u062C\u0629 \u0623\u062E\u0637\u0627\u0621 \u0645\u062A\u0634\u0627\u0628\u0643\u0629"
        }
    },
    new SlideData
    {
        Title = "\u0646\u0638\u0627\u0645 \u0627\u0644\u0637\u0644\u0628\u0627\u062A",
        Subtitle = "Order Creation Flow",
        BulletPoints = new[]
        {
            "1. \u0627\u0644\u062A\u062D\u0642\u0642 \u0645\u0646 \u0627\u0644\u0633\u0644\u0629 Basket \u0645\u0646 Redis",
            "2. \u0627\u0644\u062A\u062D\u0642\u0642 \u0645\u0646 \u0637\u0631\u064A\u0642\u0629 \u0627\u0644\u062A\u0648\u0635\u064A\u0644 \u0645\u0648\u062C\u0648\u062F\u0629 \u0648\u0645\u062A\u0627\u062D\u0629",
            "3. \u0627\u0644\u062A\u062D\u0642\u0642 \u0645\u0646 \u0627\u0644\u0639\u0645\u064A\u0644 \u0628\u0627\u0644\u0628\u062D\u062B \u0628\u0627\u0644\u0625\u064A\u0645\u064A\u0644",
            "4. \u0625\u0646\u0634\u0627\u0621 OrderItems \u0645\u0646 \u0643\u0644 \u0645\u0646\u062A\u062C \u0641\u064A \u0627\u0644\u0633\u0644\u0629",
            "5. \u062D\u0641\u0638 \u0627\u0644\u0637\u0644\u0628 \u0628\u0640 Unit of Work \u0641\u064A \u0645\u0639\u0627\u0645\u0644\u0629 \u0648\u0627\u062D\u062F\u0629"
        }
    },
    new SlideData
    {
        Title = "\u0627\u0644\u0641\u0648\u0627\u0626\u062F \u0644\u0644\u0645\u0633\u062A\u062E\u062F\u0645\u064A\u0646",
        Subtitle = "\u0627\u064A\u0647 \u0627\u0644\u0644\u064A \u0628\u064A\u0643\u0633\u0628\u0647 \u0627\u0644\u0645\u0633\u062A\u062E\u062F\u0645",
        BulletPoints = new[]
        {
            "\u0633\u0631\u0639\u0629 \u0641\u0627\u0626\u0642\u0629 \u2192 Redis Caching \u0644\u0644\u0628\u064A\u0627\u0646\u0627\u062A \u0627\u0644\u0645\u062A\u0643\u0631\u0631\u0629",
            "\u0623\u0645\u0627\u0646 \u0643\u0627\u0645\u0644 \u2192 JWT + Role-based Authorization",
            "\u062A\u062C\u0631\u0628\u0629 \u0633\u0644\u0633\u0629 \u2192 \u0633\u0644\u0629 \u0630\u0643\u064A\u0629 \u0645\u0633\u062A\u0645\u0631\u0629 \u0639\u0644\u0649 \u0627\u0644\u0633\u064A\u0631\u0641\u0631",
            "\u0645\u062A\u0648\u0627\u0641\u0642 \u0645\u0639 \u0643\u0644 \u0627\u0644\u0623\u062C\u0647\u0632\u0629 \u2192 API + CORS \u062C\u0627\u0647\u0632 \u0644\u0623\u064A Frontend"
        }
    },
    new SlideData
    {
        Title = "\u0627\u0644\u0645\u0634\u0631\u0648\u0639 \u062C\u0627\u0647\u0632 \u0644\u0644\u062A\u0648\u0633\u0639",
        Subtitle = "\u062E\u0637\u0648\u0627\u062A \u0642\u0627\u062F\u0645\u0629",
        BulletPoints = new[]
        {
            "\u062C\u0627\u0647\u0632: \u0633\u0644\u0629 + \u0637\u0644\u0628\u0627\u062A + \u0645\u0635\u0627\u062F\u0642\u0629 + API",
            "\u0627\u0644\u0642\u0627\u062F\u0645: \u062F\u0641\u0639 + \u062A\u062A\u0628\u0639 \u0634\u062D\u0646\u0627\u062A + \u0625\u0634\u0639\u0627\u0631\u0627\u062A",
            "\u0645\u062A\u062C\u0631 \u0642\u0647\u0648\u0629 \u0625\u0644\u0643\u062A\u0631\u0648\u0646\u064A \u0643\u0627\u0645\u0644 \u0645\u0646 \u0627\u0644\u0635\u0641\u0631",
            "CoffeeStore - Built with Clean Architecture"
        }
    })
);

using var doc = PresentationDocument.Create(outputPath, PresentationDocumentType.Presentation);
var presentationPart = doc.AddPresentationPart();
presentationPart.Presentation = new P.Presentation();

var slideIdList = presentationPart.Presentation.AppendChild(new SlideIdList());
var slideMasterIdList = presentationPart.Presentation.AppendChild(new SlideMasterIdList());
var slideSize = presentationPart.Presentation.AppendChild(new SlideSize { Cx = 12192000U, Cy = 6858000U });
presentationPart.Presentation.Append(new DefaultTextStyle());

var slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>();
var slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>();

var slideMaster = new SlideMaster(
    new CommonSlideData(new ShapeTree()),
    new SlideLayoutIdList(new SlideLayoutId { Id = 2147483649U, RelationshipId = slideMasterPart.GetIdOfPart(slideLayoutPart) }),
    new TextStyles(
        new TitleStyle(new A.LatinTypeface { Typeface = "Segoe UI" }),
        new BodyStyle(new A.LatinTypeface { Typeface = "Segoe UI" }),
        new OtherStyle(new A.LatinTypeface { Typeface = "Segoe UI" }))
);

slideMasterPart.SlideMaster = slideMaster;

var layoutTree = new ShapeTree(
    new P.GroupShapeProperties(new A.TransformGroup()),
    new P.Shape(
        new P.NonVisualShapeProperties(
            new P.NonVisualDrawingProperties { Id = 2U, Name = "Title" },
            new P.NonVisualShapeDrawingProperties()),
        new P.ShapeProperties(),
        new P.TextBody(new A.BodyProperties(), new A.ListStyle()))
);

slideLayoutPart.SlideLayout = new SlideLayout(layoutTree);

uint slideId = 256U;

foreach (var data in slideData)
{
    var slidePart = presentationPart.AddNewPart<SlidePart>();
    var slide = new Slide(
        new CommonSlideData(
            new ShapeTree(
                new P.GroupShapeProperties(new A.TransformGroup()),
                CreateTitleShape(data.Title, data.Subtitle),
                CreateContentShape(data.BulletPoints))
        ),
    new ColorMapOverride(new MasterColorMapping()));

    slidePart.Slide = slide;
    slidePart.AddPart(slideLayoutPart);

    slideIdList.Append(new SlideId { Id = slideId++, RelationshipId = presentationPart.GetIdOfPart(slidePart) });
}

presentationPart.Presentation.Save();

Console.WriteLine($"Presentation saved to: {outputPath}");

static P.Shape CreateTitleShape(string title, string subtitle)
{
    return new P.Shape(
        new P.NonVisualShapeProperties(
            new P.NonVisualDrawingProperties { Id = 2U, Name = "Title" },
            new P.NonVisualShapeDrawingProperties(new A.ShapeLocks { NoGrouping = true })),
        new P.ShapeProperties(),
        new P.TextBody(
            new A.BodyProperties(),
            new A.ListStyle(),
            new A.Paragraph(
                new A.ParagraphProperties(new A.LineSpacing { SpacingPercent = 100000 }),
                new A.Run(
                    new A.RunProperties { Language = "ar-SA", Bold = true, FontSize = 4000 },
                    new A.Text(title))),
            new A.Paragraph(
                new A.ParagraphProperties(),
                new A.Run(
                    new A.RunProperties { Language = "ar-SA", FontSize = 2400 },
                    new A.Text(subtitle))))
    );
}

static P.Shape CreateContentShape(string[] bulletPoints)
{
    var paragraphs = new List<A.Paragraph>();

    foreach (var point in bulletPoints)
    {
        paragraphs.Add(new A.Paragraph(
            new A.ParagraphProperties(
                new A.LineSpacing { SpacingPercent = 120000 },
                new A.BulletCharacter { Character = "\u2022" }),
            new A.Run(
                new A.RunProperties { Language = "ar-SA", FontSize = 1800 },
                new A.Text(point))));
    }

    return new P.Shape(
        new P.NonVisualShapeProperties(
            new P.NonVisualDrawingProperties { Id = 3U, Name = "Content" },
            new P.NonVisualShapeDrawingProperties(new A.ShapeLocks { NoGrouping = true })),
        new P.ShapeProperties(),
        new P.TextBody(
            new A.BodyProperties(),
            new A.ListStyle(),
            paragraphs)
    );
}

class SlideData
{
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string[] BulletPoints { get; init; }
}
