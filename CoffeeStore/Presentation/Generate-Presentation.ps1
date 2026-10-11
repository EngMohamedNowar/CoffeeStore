$ErrorActionPreference = "Stop"

$slides = @(
    @{
        Title = "CoffeeStore E-Commerce"
        Subtitle = "نظام إدارة متجر قهوة إلكتروني"
        Content = @"
<div style='text-align:right; direction:rtl;'>
<p style='font-size:28px; color:#fff; line-height:1.8;'>
منصة تجارة إلكترونية متكاملة مبنية بأحدث معماريات .NET
</p>
<br/>
<p style='font-size:18px; color:#aaa; line-height:1.6;'>
معمارية نظيفة &bull; أداء عالي &bull; قابلية توسع غير محدودة
</p>
<br/>
<div style='margin-top:30px; padding:15px; border-left:3px solid #c0392b; background:#1a1a1a;'>
<p style='font-size:16px; color:#ddd; direction:rtl; text-align:right;'>
.NET 9 &bull; Clean Architecture &bull; CQRS &bull; Unit of Work &bull; Specification Pattern
</p>
</div>
</div>
"@
    },
    @{
        Title = "ليه المعمارية دي؟"
        Subtitle = "المشاكل اللي بنحلها"
        Content = @"
<div style='text-align:right; direction:rtl;'>
<table style='width:100%; border-collapse:collapse; direction:rtl;'>
<tr style='background:#c0392b; color:#fff;'>
<th style='padding:12px; text-align:right; font-size:16px;'>المشكلة</th>
<th style='padding:12px; text-align:right; font-size:16px;'>الحل</th>
</tr>
<tr style='background:#1a1a1a;'>
<td style='padding:12px; color:#ddd; font-size:14px; border-bottom:1px solid #333;'>الكود متشابك وصعب التعديل</td>
<td style='padding:12px; color:#ddd; font-size:14px; border-bottom:1px solid #333;'>Clean Architecture - فصل الطبقات</td>
</tr>
<tr style='background:#222;'>
<td style='padding:12px; color:#ddd; font-size:14px; border-bottom:1px solid #333;'>صعوبة اختبار الكود</td>
<td style='padding:12px; color:#ddd; font-size:14px; border-bottom:1px solid #333;'>Dependency Injection + Repository Pattern</td>
</tr>
<tr style='background:#1a1a1a;'>
<td style='padding:12px; color:#ddd; font-size:14px; border-bottom:1px solid #333;'>استعلامات SQL مكررة</td>
<td style='padding:12px; color:#ddd; font-size:14px; border-bottom:1px solid #333;'>Specification Pattern</td>
</tr>
<tr style='background:#222;'>
<td style='padding:12px; color:#ddd; font-size:14px; border-bottom:1px solid #333;'>مشاكل في المعاملات المعقدة</td>
<td style='padding:12px; color:#ddd; font-size:14px; border-bottom:1px solid #333;'>Unit of Work Pattern</td>
</tr>
<tr style='background:#1a1a1a;'>
<td style='padding:12px; color:#ddd; font-size:14px;'>صعوبة إضافة ميزات جديدة</td>
<td style='padding:12px; color:#ddd; font-size:14px;'>Result Pattern + Domain-Driven Design</td>
</tr>
</table>
</div>
"@
    },
    @{
        Title = "هيكل المشروع"
        Subtitle = "4 طبقات نظيفة ومنفصلة"
        Content = @"
<div style='display:flex; gap:15px; direction:rtl;'>
<div style='flex:1; background:#c0392b; padding:20px; border-radius:8px; text-align:center;'>
<p style='color:#fff; font-size:18px; font-weight:bold;'>ECommerce.Api</p>
<p style='color:#ffcccc; font-size:12px; margin-top:8px;'>Controllers + Middleware</p>
<p style='color:#ffcccc; font-size:11px;'>JWT Auth + Swagger</p>
</div>
<div style='flex:1; background:#2980b9; padding:20px; border-radius:8px; text-align:center;'>
<p style='color:#fff; font-size:18px; font-weight:bold;'>ECommerce.Application</p>
<p style='color:#cce5ff; font-size:12px; margin-top:8px;'>Services + DTOs</p>
<p style='color:#cce5ff; font-size:11px;'>Business Logic + Validation</p>
</div>
<div style='flex:1; background:#27ae60; padding:20px; border-radius:8px; text-align:center;'>
<p style='color:#fff; font-size:18px; font-weight:bold;'>ECommerce.Domain</p>
<p style='color:#ccffcc; font-size:12px; margin-top:8px;'>Entities + Interfaces</p>
<p style='color:#ccffcc; font-size:11px;'>Domain Rules + Events</p>
</div>
<div style='flex:1; background:#8e44ad; padding:20px; border-radius:8px; text-align:center;'>
<p style='color:#fff; font-size:18px; font-weight:bold;'>ECommerce.Infrastructure</p>
<p style='color:#e8ccff; font-size:12px; margin-top:8px;'>EF Core + Redis</p>
<p style='color:#e8ccff; font-size:11px;'>Repositories + Identity</p>
</div>
</div>
<br/>
<div style='text-align:center; direction:rtl;'>
<p style='color:#aaa; font-size:14px;'>كل طبقة بتعرف على اللي تحتها بس - انفصال تام</p>
</div>
"@
    },
    @{
        Title = "Design Patterns المستخدمة"
        Subtitle = "اختيارات مدروسة لكل نمط"
        Content = @"
<div style='direction:rtl;'>
<div style='background:#1a1a1a; padding:15px; margin-bottom:10px; border-right:3px solid #c0392b;'>
<p style='color:#fff; font-size:16px; font-weight:bold;'>Repository Pattern</p>
<p style='color:#aaa; font-size:13px; margin-top:5px;'>يفصل طبقة البيانات عن البزنس لوجيك - تقدر تغير الـ DB من غير ما تلمس أي كود تاني</p>
</div>
<div style='background:#1a1a1a; padding:15px; margin-bottom:10px; border-right:3px solid #c0392b;'>
<p style='color:#fff; font-size:16px; font-weight:bold;'>Unit of Work</p>
<p style='color:#aaa; font-size:13px; margin-top:5px;'>بيضمن إن كل العمليات على قاعدة البيانات تتم في معاملة واحدة - مفيش partial saves</p>
</div>
<div style='background:#1a1a1a; padding:15px; margin-bottom:10px; border-right:3px solid #c0392b;'>
<p style='color:#fff; font-size:16px; font-weight:bold;'>Specification Pattern</p>
<p style='color:#aaa; font-size:13px; margin-top:5px;'>الـ queries بقابلة لإعادة الاستخدام والتركيب - مفيش SQL مكرر</p>
</div>
<div style='background:#1a1a1a; padding:15px; margin-bottom:10px; border-right:3px solid #c0392b;'>
<p style='color:#fff; font-size:16px; font-weight:bold;'>Result Pattern</p>
<p style='color:#aaa; font-size:13px; margin-top:5px;'>بدل ما نرمي Exceptions في كل مكان - بنرجع Result واضح فيه Success أو Failure</p>
</div>
<div style='background:#1a1a1a; padding:15px; border-right:3px solid #c0392b;'>
<p style='color:#fff; font-size:16px; font-weight:bold;'>Dependency Injection</p>
<p style='color:#aaa; font-size:13px; margin-top:5px;'>كل خدمة بتاخد اعتمادياتها من بره - اختبار سهل وصيانة أسهل</p>
</div>
</div>
"@
    },
    @{
        Title = "معالجة الأخطاء"
        Subtitle = "Result Pattern بالتفصيل"
        Content = @"
<div style='direction:rtl;'>
<div style='background:#1a1a1a; padding:20px; border-radius:8px; margin-bottom:15px;'>
<p style='color:#fff; font-size:16px; font-weight:bold; margin-bottom:10px;'>بدل كده</p>
<pre style='color:#ff6b6b; font-size:13px; background:#111; padding:15px; border-radius:5px; direction:ltr; text-align:left;'>
try { ... }
catch (Exception ex) 
{ 
    throw new Exception("Error"); 
}</pre>
</div>
<div style='background:#1a1a1a; padding:20px; border-radius:8px;'>
<p style='color:#fff; font-size:16px; font-weight:bold; margin-bottom:10px;'>بنعمل كده</p>
<pre style='color:#4ecdc4; font-size:13px; background:#111; padding:15px; border-radius:5px; direction:ltr; text-align:left;'>
var result = Order.Create(...);
if (result.IsFailure)
    return Result&lt;OrderToReturn&gt;.Fail(result.Errors);

return Result&lt;OrderToReturn&gt;.Ok(...);</pre>
</div>
<br/>
<div style='background:#222; padding:15px; border-radius:5px; direction:rtl;'>
<p style='color:#aaa; font-size:13px;'>
كل Error له Code + Description + Type (NotFound, Validation, Conflict)
</p>
</div>
</div>
"@
    },
    @{
        Title = "نظام الطلبات"
        Subtitle = "Order Creation Flow"
        Content = @"
<div style='direction:rtl;'>
<div style='background:#1a1a1a; padding:15px; border-radius:8px; margin-bottom:10px; border-right:3px solid #c0392b;'>
<p style='color:#fff; font-size:14px;'>1. التحقق من السلة (Basket)</p>
<p style='color:#aaa; font-size:12px; margin-top:5px;'>بنجيب السلة من Redis ونتأكد إنها مش فاضية</p>
</div>
<div style='background:#1a1a1a; padding:15px; border-radius:8px; margin-bottom:10px; border-right:3px solid #2980b9;'>
<p style='color:#fff; font-size:14px;'>2. التحقق من طريقة التوصيل</p>
<p style='color:#aaa; font-size:12px; margin-top:5px;'>بنتأكد إن طريقة التوصيل موجودة ومتاحة</p>
</div>
<div style='background:#1a1a1a; padding:15px; border-radius:8px; margin-bottom:10px; border-right:3px solid #27ae60;'>
<p style='color:#fff; font-size:14px;'>3. التحقق من العميل</p>
<p style='color:#aaa; font-size:12px; margin-top:5px;'>بندور على العميل بالإيميل</p>
</div>
<div style='background:#1a1a1a; padding:15px; border-radius:8px; margin-bottom:10px; border-right:3px solid #8e44ad;'>
<p style='color:#fff; font-size:14px;'>4. إنشاء OrderItems</p>
<p style='color:#aaa; font-size:12px; margin-top:5px;'>بنحول كل منتج في السلة لطلب</p>
</div>
<div style='background:#1a1a1a; padding:15px; border-radius:8px; border-right:3px solid #f39c12;'>
<p style='color:#fff; font-size:14px;'>5. حفظ الطلب</p>
<p style='color:#aaa; font-size:12px; margin-top:5px;'>Unit of Work بيحفظ كل حاجة في معاملة واحدة</p>
</div>
</div>
"@
    },
    @{
        Title = "الفوائد للمستخدمين"
        Subtitle = "ايه اللي بيكسبه المستخدم"
        Content = @"
<div style='display:grid; grid-template-columns:1fr 1fr; gap:15px; direction:rtl;'>
<div style='background:#1a1a1a; padding:20px; border-radius:8px; text-align:center;'>
<p style='font-size:32px; color:#c0392b;'>⚡</p>
<p style='color:#fff; font-size:16px; font-weight:bold; margin-top:10px;'>سرعة فائقة</p>
<p style='color:#aaa; font-size:13px; margin-top:5px;'>Redis Caching للبيانات المتكررة</p>
</div>
<div style='background:#1a1a1a; padding:20px; border-radius:8px; text-align:center;'>
<p style='font-size:32px; color:#27ae60;'>🔒</p>
<p style='color:#fff; font-size:16px; font-weight:bold; margin-top:10px;'>أمان كامل</p>
<p style='color:#aaa; font-size:13px; margin-top:5px;'>JWT + Role-based Authorization</p>
</div>
<div style='background:#1a1a1a; padding:20px; border-radius:8px; text-align:center;'>
<p style='font-size:32px; color:#2980b9;'>🛒</p>
<p style='color:#fff; font-size:16px; font-weight:bold; margin-top:10px;'>تجربة سلسة</p>
<p style='color:#aaa; font-size:13px; margin-top:5px;'>سلة ذكية مستمرة على السيرفر</p>
</div>
<div style='background:#1a1a1a; padding:20px; border-radius:8px; text-align:center;'>
<p style='font-size:32px; color:#8e44ad;'>📱</p>
<p style='color:#fff; font-size:16px; font-weight:bold; margin-top:10px;'>متوافق مع كل الأجهزة</p>
<p style='color:#aaa; font-size:13px; margin-top:5px;'>API + CORS جاهز لأي Frontend</p>
</div>
</div>
"@
    },
    @{
        Title = "نقاط القوة التقنية"
        Subtitle = "Technical Highlights"
        Content = @"
<div style='direction:rtl;'>
<div style='background:#1a1a1a; padding:15px; margin-bottom:8px; border-radius:5px; display:flex; justify-content:space-between;'>
<p style='color:#fff; font-size:14px;'>Domain Validation</p>
<p style='color:#4ecdc4; font-size:13px;'>كل قواعد العمل في الـ Domain Entities</p>
</div>
<div style='background:#1a1a1a; padding:15px; margin-bottom:8px; border-radius:5px; display:flex; justify-content:space-between;'>
<p style='color:#fff; font-size:14px;'>Specification Pattern</p>
<p style='color:#4ecdc4; font-size:13px;'>استعلامات قابلة لإعادة الاستخدام</p>
</div>
<div style='background:#1a1a1a; padding:15px; margin-bottom:8px; border-radius:5px; display:flex; justify-content:space-between;'>
<p style='color:#fff; font-size:14px;'>AutoMapper</p>
<p style='color:#4ecdc4; font-size:13px;'>تحويل تلقائي بين الـ Entities و DTOs</p>
</div>
<div style='background:#1a1a1a; padding:15px; margin-bottom:8px; border-radius:5px; display:flex; justify-content:space-between;'>
<p style='color:#fff; font-size:14px;'>Result Pattern</p>
<p style='color:#4ecdc4; font-size:13px;'>معالجة أخطاء نظيفة بدون Exceptions</p>
</div>
<div style='background:#1a1a1a; padding:15px; margin-bottom:8px; border-radius:5px; display:flex; justify-content:space-between;'>
<p style='color:#fff; font-size:14px;'>Unit of Work</p>
<p style='color:#4ecdc4; font-size:13px;'>معاملات ذرية على قاعدة البيانات</p>
</div>
<div style='background:#1a1a1a; padding:15px; border-radius:5px; display:flex; justify-content:space-between;'>
<p style='color:#fff; font-size:14px;'>Dependency Injection</p>
<p style='color:#4ecdc4; font-size:13px;'>حقن تلقائي لكل الخدمات</p>
</div>
</div>
"@
    },
    @{
        Title = "المشروع جاهز للتوسع"
        Subtitle = "خطوات قادمة"
        Content = @"
<div style='direction:rtl;'>
<div style='background:linear-gradient(135deg, #1a1a1a, #2d0a0a); padding:20px; border-radius:8px; margin-bottom:15px; border:1px solid #c0392b;'>
<p style='color:#fff; font-size:18px; font-weight:bold;'>المشروع بيحل ايه؟</p>
<p style='color:#aaa; font-size:14px; margin-top:10px; line-height:1.8;'>
متجر قهوة إلكتروني كامل من الصفر - سلة، طلبات، دفع، تتبع، وتوصيل
</p>
</div>
<div style='display:grid; grid-template-columns:1fr 1fr; gap:10px; direction:rtl;'>
<div style='background:#1a1a1a; padding:15px; border-radius:5px;'>
<p style='color:#fff; font-size:14px; font-weight:bold;'>جاهز</p>
<p style='color:#aaa; font-size:12px; margin-top:5px;'>سلة + طلبات + مصادقة + API</p>
</div>
<div style='background:#1a1a1a; padding:15px; border-radius:5px;'>
<p style='color:#fff; font-size:14px; font-weight:bold;'>القادم</p>
<p style='color:#aaa; font-size:12px; margin-top:5px;'>دفع + تتبع شحنات + إشعارات</p>
</div>
</div>
<br/>
<div style='text-align:center; margin-top:30px;'>
<p style='color:#c0392b; font-size:20px; font-weight:bold;'>CoffeeStore</p>
<p style='color:#666; font-size:14px;'>Built with Clean Architecture</p>
</div>
</div>
"@
    }
)

$currentSlide = 0

function Show-Slide {
    param($index)
    $slide = $slides[$index]
    $total = $slides.Count
    $progress = [math]::Round((($index + 1) / $total) * 100)
    
    Clear-Host
    Write-Host ""
    Write-Host ("=" * 80) -ForegroundColor DarkGray
    Write-Host ""
    
    Write-Host "  $($slide.Title)" -ForegroundColor Red -NoNewline
    Write-Host ""
    Write-Host "  $($slide.Subtitle)" -ForegroundColor DarkRed
    Write-Host ""
    Write-Host ("-" * 80) -ForegroundColor DarkGray
    Write-Host ""
    
    Write-Host $slide.Content
    
    Write-Host ""
    Write-Host ("-" * 80) -ForegroundColor DarkGray
    Write-Host ""
    Write-Host "  Slide $($index + 1) of $total  |  Progress: $progress%" -ForegroundColor DarkGray
    Write-Host ""
    
    Write-Host "  [N]ext  |  [P]revious  |  [Q]uit  |  [G]o to slide" -ForegroundColor Yellow
    Write-Host ""
}

Show-Slide $currentSlide

while ($true) {
    $key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    
    switch ($key.Character) {
        'n' { 
            if ($currentSlide -lt $slides.Count - 1) { 
                $currentSlide++ 
                Show-Slide $currentSlide
            }
        }
        'N' { 
            if ($currentSlide -lt $slides.Count - 1) { 
                $currentSlide++ 
                Show-Slide $currentSlide
            }
        }
        'p' { 
            if ($currentSlide -gt 0) { 
                $currentSlide-- 
                Show-Slide $currentSlide
            }
        }
        'P' { 
            if ($currentSlide -gt 0) { 
                $currentSlide-- 
                Show-Slide $currentSlide
            }
        }
        'q' { break }
        'Q' { break }
        'g' {
            Write-Host "  Enter slide number (1-$($slides.Count)): " -ForegroundColor Cyan -NoNewline
            $input = Read-Host
            if ($input -match '^\d+$' -and [int]$input -ge 1 -and [int]$input -le $slides.Count) {
                $currentSlide = [int]$input - 1
                Show-Slide $currentSlide
            }
        }
        'G' {
            Write-Host "  Enter slide number (1-$($slides.Count)): " -ForegroundColor Cyan -NoNewline
            $input = Read-Host
            if ($input -match '^\d+$' -and [int]$input -ge 1 -and [int]$input -le $slides.Count) {
                $currentSlide = [int]$input - 1
                Show-Slide $currentSlide
            }
        }
    }
    
    if ($key.Character -eq 'q' -or $key.Character -eq 'Q') { break }
}

Write-Host ""
Write-Host "  Presentation ended. Goodbye!" -ForegroundColor Green
Write-Host ""
