using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ShoeStores.Objects
{
  public class Store
  {
    private int _id;
    private string _store_name;

    public Store(string StoreName, int Id = 0)
    {
      _id = Id;
      _store_name = StoreName;
    }

    public override bool Equals(System.Object otherStore)
    {
      if(!(otherStore is Store))
      {
        return false;
      }
      else
      {
        var newStore = (Store) otherStore;
        bool idEquality = this.GetId() == newStore.GetId();
        bool nameEquality = this.GetName() == newStore.GetName();
        return (idEquality && nameEquality);
      }
    }

    public int GetId()
    {
      return _id;
    }

    public string GetName()
    {
      return _store_name;
    }

    public static List<Store> GetAll()
    {
      List<Store> AllStores = new List<Store>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      var cmd = new SqlCommand("SELECT * FROM stores", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int StoreId = rdr.GetInt32(0);
        string StoreName = rdr.GetString(1);

        var newStore = new Store(StoreName, StoreId);
        AllStores.Add(newStore);
      }

      if(rdr != null)
      {
        rdr.Close();
      }

      if(conn != null)
      {
        conn.Close();
      }

      return AllStores;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      var cmd = new SqlCommand("INSERT INTO stores (name) OUTPUT INSERTED.id VALUES (@StoreName);", conn);

      var nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@StoreName";
      nameParameter.Value = this.GetName();

      cmd.Parameters.Add(nameParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }

      if(rdr != null)
      {
        rdr.Close();
      }

      if(conn != null)
      {
        conn.Close();
      }
    }

    public static Store Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      var cmd = new SqlCommand("SELECT * FROM stores WHERE id = @StoreId;", conn);
      var storeIdParameter = new SqlParameter();
      storeIdParameter.ParameterName = "@StoreId";
      storeIdParameter.Value = id.ToString();
      cmd.Parameters.Add(storeIdParameter);
      rdr = cmd.ExecuteReader();

      int foundStoreId = 0;
      string foundStoreName = null;

      while(rdr.Read())
      {
        foundStoreId = rdr.GetInt32(0);
        foundStoreName = rdr.GetString(1);
      }

      var foundStore = new Store(foundStoreName, foundStoreId);

      if(rdr != null)
      {
        rdr.Close();
      }

      if(conn != null)
      {
        conn.Close();
      }

      return foundStore;
    }

    public void AddBrand(Brand newBrand)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO brands_stores (brand_id, store_id) VALUES (@BrandId, @StoreId);", conn);

      SqlParameter brandIdParameter = new SqlParameter();
      brandIdParameter.ParameterName = "@BrandId";
      brandIdParameter.Value = newBrand.GetId();
      cmd.Parameters.Add(brandIdParameter);

      SqlParameter storeIdParameter = new SqlParameter();
      storeIdParameter.ParameterName = "@StoreId";
      storeIdParameter.Value = this.GetId();
      cmd.Parameters.Add(storeIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Brand> GetBrands()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT brands.* FROM stores JOIN brands_stores ON (stores.id = brands_stores.store_id) JOIN brands ON (brands_stores.brand_id = brands.id) WHERE stores.id = @StoreId;", conn);

      SqlParameter storeIdParameter = new SqlParameter();
      storeIdParameter.ParameterName = "@StoreId";
      storeIdParameter.Value = this.GetId();
      cmd.Parameters.Add(storeIdParameter);

      rdr = cmd.ExecuteReader();

      List<Brand> brands = new List<Brand> {};

      while (rdr.Read())
      {
        int thisBrandId = rdr.GetInt32(0);
        string brandName = rdr.GetString(1);
        Brand foundBrand = new Brand(brandName, thisBrandId);
        brands.Add(foundBrand);
      }

      if (rdr != null)
      {
        rdr.Close();
      }

      if (conn != null)
      {
        conn.Close();
      }

      return brands;
    }

    public void Update(string newName)
   {
     _store_name = newName;
     SqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = new SqlCommand("UPDATE stores SET name = @NewName WHERE id = @StoreId;", conn);

     var newNameParameter = new SqlParameter();
     newNameParameter.ParameterName = "@NewName";
     newNameParameter.Value = newName;
     cmd.Parameters.Add(newNameParameter);

     SqlParameter storeIdParameter = new SqlParameter();
     storeIdParameter.ParameterName = "@StoreId";
     storeIdParameter.Value = this.GetId();
     cmd.Parameters.Add(storeIdParameter);

     cmd.ExecuteNonQuery();

     if (conn != null)
     {
       conn.Close();
     }
   }

   public void Delete()
   {
     SqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = new SqlCommand("DELETE FROM stores WHERE id = @StoreId;", conn);

     var storeIdParameter = new SqlParameter();
     storeIdParameter.ParameterName = "@StoreId";
     storeIdParameter.Value = this.GetId();

     cmd.Parameters.Add(storeIdParameter);
     cmd.ExecuteNonQuery();

     if (conn != null)
     {
       conn.Close();
     }
   }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM stores", conn);
      cmd.ExecuteNonQuery();
    }
  }
}
