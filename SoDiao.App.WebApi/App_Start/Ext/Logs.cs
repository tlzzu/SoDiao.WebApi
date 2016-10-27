using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoDiao.App.WebApi
{
    /// <summary>
    /// 系统日志--已知错误不用写日志，未知错误才需要写日志
    /// </summary>
    public  class Logs
    {
        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="ex"></param>
        public static void Debug(object ex)
        {
            
        }
        /// <summary>
        /// 提醒
        /// </summary>
        /// <param name="ex"></param>
        public static void Warn(object ex)
        {
            
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(object ex)
        {
           
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="ex"></param>
        public static void Info(object ex)
        {
           
        }
        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Debug(object message, Exception ex)
        {
           
        }
        /// <summary>
        /// 提醒
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Warn(object message, Exception ex)
        {
           
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Error(object message, Exception ex)
        {
           
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Info(object message, Exception ex)
        {
           
        }
    }
}