using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Examples.Chapter1.DbLogger;
using Examples;
using Microsoft.Extensions.Logging;
using String = System.String;
using Unit = System.ValueTuple;

namespace Playground.WithLINQ.DbLogger
{
   public class LogMessage { }

   public class DbLogger_PyramidOfDoom
   {
      string connString;

      public void Log(LogMessage message)
         => Instrumentation.Time("CreateLog"
            , () => ConnectionHelper.Connect(connString
               , c => c.Execute("sp_create_log"
                  , message, commandType: CommandType.StoredProcedure)));
   }

   public class DbLogger
   {
      Middleware<SqlConnection> Connect = null;
      Func<String, Middleware<Unit>> Time = null;
      Func<String, Middleware<Unit>> Trace = null;

      public DbLogger(ConnectionString connString, ILogger log)
      {
         //Connect = f => ConnectionHelper.Connect(connString, f);
         this.Connect = new Middleware<SqlConnection>(f => { return ConnectionHelper.Connect<dynamic>(connString, f); });
         this.Time = new Func<String, Middleware<Unit>>(op =>
             {
                 return new Middleware<Unit>(f => Instrumentation.Time(log, op, f.ToNullary()));
             });
         this.Trace = op => f => Instrumentation.Trace(log, op, f.ToNullary());
      }

      Middleware<SqlConnection> BasicPipeline =>
         from _ in this.Time("InsertLog")
         from conn in this.Connect
         select conn;

      // demonstrates running a pipeline by directly passing a 
      // continuation; that is, without using Run
      // this will produce a dynamic value, which will be unsafely cast to int
      public int Dynamic_Log(LogMessage message) 
         => this.BasicPipeline(conn
            => conn.Execute("sp_create_log", message
               , commandType: CommandType.StoredProcedure));

      //public void Log_1(LogMessage message) =>
      //   Connect(conn => conn.Execute("sp_create_log", message
      //      , commandType: CommandType.StoredProcedure));

      public void Log_1(LogMessage message)
      {
          this.Connect.Invoke(new Func<SqlConnection, dynamic>( sqlConn => sqlConn.Execute("sp_create_log", message
              , commandType: CommandType.StoredProcedure)));
      }

      public void LogBook(LogMessage message) => (
          from conn in this.Connect
              select conn.Execute("sp_create_log", message, commandType: CommandType.StoredProcedure)
          ).Run();
      

      public void Log_2(LogMessage message) =>
          this.Connect
            .Map(conn => conn.Execute("sp_create_log", message
               , commandType: CommandType.StoredProcedure))
            .Run();

      public void Log(LogMessage message) => (
         from _ in this.Time("InsertLog")
         from conn in this.Connect
         select conn.Execute("sp_create_log", message
                            , commandType: CommandType.StoredProcedure)
      ).Run();

      public void DeleteOldLogs() => (
         from _ in this.Time("DeleteOldLogs")
         from conn in this.Connect
         select conn.Execute("DELETE [Logs] WHERE [Timestamp] < @upTo"
                            , new { upTo = 7.Days().Ago() })
      ).Run();

      public IEnumerable<LogMessage> GetLogs(DateTime since) => (
         from _    in this.Trace("GetLogs")
         from __   in this.Time("GetLogs")
         from conn in this.Connect
         select conn.Query<LogMessage>(@"SELECT * 
            FROM [Logs] WHERE [Timestamp] > @since", new { since = since })
      ).Run();
   }

   public class Orders
   {
      ConnectionString connString = null;

      Middleware<SqlConnection> Connect
         => f => ConnectionHelper.Connect(connString, f);

      Middleware<SqlTransaction> Transact(SqlConnection conn)
         => f => ConnectionHelper.Transact(conn, f);

      public void DeleteOrder(Guid id) => this.DeleteOrder(new { Id = id }).Run();

      SqlTemplate deleteLines = "DELETE OrderLines WHERE OrderId = @Id";
      SqlTemplate deleteOrder = "DELETE Orders WHERE OrderId = @Id";

      Middleware<int> DeleteOrder(object param) =>
         from conn in this.Connect
         from tran in this.Transact(conn)
         select conn.Execute(this.deleteLines, param, tran)
              + conn.Execute(this.deleteOrder, param, tran);
   }
}
