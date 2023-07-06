using PefectMoney.Shared.Utility.ResultUtil;
using System.Text;

namespace PefectMoney.Shared.Utility.ResultUtil;


/// <summary>
/// اکستنشن هایی جهت راحت کردن استفاده از
/// resultOperation
/// </summary>
public static class UtilityResultExtension
{

    public static string GetString(this IEnumerable<string> strings)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var str in strings)
        {
            sb.AppendLine(str);
        }
        return sb.ToString();   
    }
    /// <summary>
    /// ساختن یک نتیجه عملیات موفق به صورت جنریک
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ResultOperation<T> ToSuccessResult<T>(this T entity)
    {
        return ResultOperation<T>.ToSuccessResult(entity);
    }
     public static ResultOperation<T> ToSuccessResult<T>(this T entity, IEnumerable<string> messages)
    {
        return ResultOperation<T>.ToSuccessResult(messages, entity);
    }
    /// <summary>
    /// ساختن یک نتیجه عملیات موفق به صورت جنریک به همراه پیام
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ResultOperation<T> ToSuccessResult<T>(this T entity, string message)
    {
        return ResultOperation<T>.ToSuccessResult(message, entity);
    }

    public static ResultOperation<T> ToFailedResult<T>(this T entity)
    {
        return ResultOperation<T>.ToFailedResult();
    }
    public static ResultOperation<T> ToFailedResult<T>(this T entity, string message)
    {
        return ResultOperation<T>.ToFailedResult(message);
    }
    public static ResultOperation<T> ToFailedResult<T>(this T entity, IEnumerable<string> message)
    {
        return ResultOperation<T>.ToFailedResult(message);
    }
    public static ResultOperation<object> ToFailedResult(this object entity, string message)
    {
        return ResultOperation<object>.ToFailedResult(message);
    }
    public static ResultOperation<object> ToFailedResult(this object entity)
    {
        return ResultOperation<object>.ToFailedResult();
    }
}
