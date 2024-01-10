// a class that wraps the idea of a query
using System.Dynamic;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using SQLitePCL;


namespace Tailwind.Data;

public interface ICommand{
  Task<dynamic> Execute();
}

public class Transaction : IDisposable
{
  NpgsqlConnection _conn { get; set; }
  NpgsqlTransaction _tx { get; set; }
  bool _shouldCommit { get; set; } = true;
  public Transaction()
  {
    string _connectionString = Viper.Config().Get("DATABASE_URL");
    if(String.IsNullOrEmpty(_connectionString)){
      throw new InvalidOperationException("You must set a DATABASE_URL environment variable or call Viper.Config()");
    }
    _conn = new NpgsqlConnection(_connectionString);
    _conn.Open();
    _tx = _conn.BeginTransaction();
  }
  public void Commit(){
    _tx.Commit();
  }
  public void Rollback(){
    _tx.Rollback();
  }
  public dynamic Raw(string sql, object o)
  {
    var cmd = new NpgsqlCommand(sql).AddParams(o);
    return Run(cmd);
  }
  public dynamic Raw(string sql)
  {
    var cmd = new NpgsqlCommand(sql);
    return Run(cmd);
  }
  public IList<dynamic> Select(string table)
  {
    var sql = $"select * from {table} limit 1000";
    var cmd = new NpgsqlCommand(sql);
    return Run(cmd);
  }
  public IList<dynamic> Select(string table, object where)
  {
    var sql = $"select * from {table}";
    var cmd = new NpgsqlCommand(sql).Where(where).Limit(1000);
    return Run(cmd);
  }
  public dynamic First(string table, object where)
  {
    var sql = $"select * from {table}";
    var cmd = new NpgsqlCommand(sql).Where(where);
    IList<dynamic> results = Run(cmd);
    if(results == null){
      return null;
    }else{
      return results[0];
    }
  }

  public int Insert(string table, object o)
  {
    var expando = o.ToExpando();
    var values = (IDictionary<string, object>)expando;
    var sql = $"insert into {table} ({string.Join(", ", values.Keys)}) values ({string.Join(", ", values.Keys.Select(k => $"@{k}"))}) returning id;";
    var cmd = new NpgsqlCommand(sql).AddParams(o);
    return Run(cmd);
  }

  public int Update(string table, object settings, object where)
  {
     
    var settingsExpando = settings.ToExpando();
    var dSettings = (IDictionary<string, object>)settingsExpando;
    var sql = $"update {table} set {string.Join(",", dSettings.Keys.Select(k => $"{k}=@{k}"))}";
    var cmd = new NpgsqlCommand(sql).AddParams(settings);
    cmd.Where(where);
    return Run(cmd);
  }

  public int Delete(string table, object where)
  {
    var expando = where.ToExpando();
    var dict = (IDictionary<string, object>)expando;
    var sql = $"delete from {table}";
    var cmd = new NpgsqlCommand(sql);
    if(dict.IsNullOrEmpty()){
      throw new InvalidOperationException("You must provide a where clause otherwise you'll delete everything. If that's what you want, run it Raw.");
    }
    cmd.Where(dict);
    return Run(cmd);
  }

  dynamic Run(NpgsqlCommand cmd)
  {
    cmd.Connection = _conn;
    cmd.Transaction = _tx;
    Console.WriteLine(cmd.CommandText);
    try{
      if(cmd.CommandText.Contains("select") || cmd.CommandText.Contains("with")){
        using(var rdr = cmd.ExecuteReader()){
          var results = rdr.ToExpandoList();
          return results;
        }
      }
      if(cmd.CommandText.Contains("returning")){
        var result = cmd.ExecuteScalar();
        if(result is null){
          return 0; //TODO: this is a hack
        }else{
          return result;
        }
      }else{
        var recordsAffected = cmd.ExecuteNonQuery();
        return recordsAffected;
      }
    }catch(Exception ex){
      _tx.Rollback();
      _shouldCommit = false;
      throw ex;
    }

  }
  public void Dispose()
  {
    if(_shouldCommit){
      _tx.Commit();
    }
    _conn.Close();
  }
}