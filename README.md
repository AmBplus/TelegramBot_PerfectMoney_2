# TelegramBot_PerfectMoney
# ExternalServiceHandler ---[Microservice External Handler](https://link-url-here.org](https://github.com/AmBplus/ExternalServiceHandler))

ربات صرافی آنلاین پرفکت مانی در تلگرام 

موارد کلیدی پروژه
- [x] Asp.net Core
- [x] Telegram Bot
- [x] Mysql
- [x] CQRS
- [x] Rest 
- [x] MediatR
- [x] Publish Message
- [x] Connect To External Api
- [x] Result Pattern
- [x] Clean Architecture
- [x] Perfect Monney
- [x] Zibal
- [x] Finotech
- [x] Web Hook
- [x] EF Core
- [x] Microservice Concept
- [x] Modular Design


این پروژه خود شامل دو پروژه هست ، که یکی به صورت مونولوتیک و دیگری به صورت میکروسرویس پیاده سازی شده که دو سولوشن متفاوت قرار گرفته اند
لینک پروژه میکروسرویس 


[Microservice External Handler](https://link-url-here.org](https://github.com/AmBplus/ExternalServiceHandler))




این برنامه یک برنامه تجاری بوده که با گرفتن تلفن از شخص کار خودش را شروع میکند ، به صورتی که در بات تلگرام از شما درخواست میشود با برنامه شماره خود راه به اشتراک بگذارید سپس شما به قابلیت برنامه دسترسی داشت 
که از جمله قابلیت های ، چک کردن اینکه شماره شما با کارت حساب بانکی شما تطابق یا خیر که توسط سامانه فینوتک صورت میگیرد ، علاوه بر آن میتواند با خرید محصول لینک پرداخت را از زیبال گرفته و برای شما در محیط تلگرام نمایش دهد
در قسمت ادمین شما امکان توقف فروش ، شروع فروش ، بن کردن کاربر ، دیدن کاربران وغیر را دارید . 
همانجور که میدانید در ایران ما به علت فیلرینگ امکان ران کردن ربات را در سرور های معمولی که آزاد از فیلترینگ نیستن را نداریم ، فلذا امکان ارتباط مستقیم با درگاه پرداخت و غیره که نیاز به آی پی ایران دارند نداریم ، بدین منظور این پروژه به دو قسمت تبدیل شده ، که قسمت اول همین پروژه ای که مشاهده میکنید که شامل یک ربات تلگرام با معماری تمیز و با استفاده انتیتی فریم ورک و cqrs  به صورت وب هوک پیاده سازی شده ، که وظیفه آن ساختن ظاهر بر کاربر و همچنین سرویس های اولیه برنامه میباشید ، ولی برای کارهایی مثل گرفتن لینک درگاه پرداخت و همچنین احراز هویت نیاز دارد به برنامه دیگر متصل شده و به صورت Rest با آن برنامه ارتباط بگیرد ، به این صورت درخواست را به درگاه پرداخت یا ... ارسال میکند و نتیجه را برمیگرداند و در برنامه تلگرام دوباره نتیجه بررسی شده و کار های لازم انجام میشود .








