using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ReportDataBuilder.SimpleLogging
{
    public static class QueryBuilder
    {
        public static string CheckTableExisits(string database)
        {
            return $"SELECT distinct TABLE_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = '{database}';";
        }
        public static string CreateErrorTable()
        {
            return "CREATE TABLE `errorlog` ( \r\n" +
                  "`DBID` int(10) unsigned NOT NULL AUTO_INCREMENT, \r\n" +
                  "`Application` varchar(50) DEFAULT NULL, \r\n" +
                  "`Logged` datetime DEFAULT NULL, \r\n" +
                  "`Level` varchar(50) DEFAULT NULL, \r\n" +
                  "`Message` LONGTEXT DEFAULT NULL, \r\n" +
                  "`Logger` varchar(250) DEFAULT NULL, \r\n" +
                  "`Callsite` LONGTEXT DEFAULT NULL, \r\n" +
                  "`Exception` LONGTEXT DEFAULT NULL, \r\n" +
                  "PRIMARY KEY(`DBID`) \r\n" +
                  ") ENGINE = InnoDB AUTO_INCREMENT = 2 DEFAULT CHARSET = utf8; ";
            ;
        }

        public static string CreateTimeTable()
        {
            return "CREATE TABLE `applicationlog` (\r\n  " +
                "`DBID` INT NOT NULL AUTO_INCREMENT,\r\n  " +
                "`Application` VARCHAR(45) NOT NULL,\r\n  " +
                "`Office` VARCHAR(45) NOT NULL,\r\n  " +
                "`Token` INT NOT NULL,\r\n  " +
                "`Start` DATETIME NOT NULL,\r\n  " +
                "`Stop` DATETIME NOT NULL,\r\n  " +
                "PRIMARY KEY (`DBID`));" +
                "\r\n";
        }

        public static string InsertErrorLog(
            string ApplicationName,
            string Level,
            string Message,
            string LoggerName,
            string Callsite,
            string ExceptionType)
        {

            return "INSERT INTO `errorlog`\r\n" +
                "(" +
                "`Application`,\r\n" +
                "`Logged`,\r\n" +
                "`Level`,\r\n" +
                "`Message`,\r\n" +
                "`Logger`,\r\n" +
                "`Callsite`,\r\n" +
                "`Exception`\r\n" +
                ")\r\n" +
                "VALUES\r\n" +
                "(\r\n" +
                $"'{ApplicationName.Replace("'", "")}',\r\n" +
                $"NOW(),\r\n" +
                $"'{Level.Replace("'", "")}',\r\n" +
                $"'{Message.Replace("'", "")}',\r\n" +
                $"'{LoggerName.Replace("'", "")}',\r\n" +
                $"'{Callsite.Replace("'", "")}',\r\n" +
                $"'{ExceptionType.Replace("'", "")}'" +
                ");";

        }
        public static string InsertTimeLog(
           string ApplicationName,
           string Office,
           int Token)
        {

            return "INSERT INTO `applicationlog`\r\n" +
                "(\r\n" +
                "`Application`,\r\n" +
                "`Office`,\r\n" +
                "`Token`,\r\n" +
                "`Start`,\r\n" +
                "`Stop`)\r\n" +
                "VALUES\r\n" +
                "(\r\n" +
                $"'{ApplicationName}',\r\n" +
                $"'{Office}',\r\n" +
                $"{Token},\r\n" +
                "NOW(),\r\n" +
                "NOW()\r\n" +
                $");";

        }
        public static string UpdateTimeLog(
          long id)
        {
            return "UPDATE `applicationlog`\r\n" +
                "SET\r\n" +
                "`Stop` = NOW()\r\n" +
                $"WHERE `DBID` = {id};";

        }
    }
}
