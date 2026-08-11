namespace Origami.Identity.Api.Core.Data
{
    using Microsoft.IdentityModel;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;

    using static System.Net.Mime.MediaTypeNames;
    using Origami.Identity.Api.Core.Infrastructure;
    using Module = Infrastructure.Module;
    using Origami.Identity.Api.Core.Infrastructure.Security.Request;
    using Origami.Identity.Api.Core.Infrastructure.Security.Response;

    public class SecurityData
    {

        private readonly string _connectionString;

        public SecurityData(DataHelper dataHelper)
        {
            _connectionString = dataHelper.ConnectionString;
        }
        public bool CreateRecoveryCode(RecoveryCodes request)
        {
            bool response;

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;


            try
            {
                connection.Open();
                command = new SqlCommand
                {
                    CommandText = DataObjects.Usp_Security_RecoveryCodes_INS,
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                command.Parameters.Add(new SqlParameter("@RecoveryCode", SqlDbType.VarChar) { SqlValue = request.RecoveryCode });
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { SqlValue = request.Email });
                command.Parameters.Add(new SqlParameter("@ValidityDate", SqlDbType.DateTime) { SqlValue = request.ValidityDate });

                var responseInsert = command.ExecuteNonQuery();

                response = true;

            }
            catch (Exception)
            {
                response = false;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return response;
        }

        public RecoveryCodes GetRecoveryCode(string email, string code)
        {
            RecoveryCodes response = null;
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_RecoveryCodes_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@RecoveryCode", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(code) });
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(email) });
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        response = new RecoveryCodes();
                        response.Email = reader.GetString("Email");
                        response.RecoveryCode = reader.GetString("RecoveryCode");
                        response.ValidityDate = reader.GetDateTime("ValidityDate");
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public Role GetRoleByUserId(string userId)
        {
            Role response = null;
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_RoleByUserId_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        response = new Role();
                        response.Id = reader.GetString("Id");
                        response.Name = reader.GetString("Name");
                        response.RoleId = reader.GetInt32("RoleId");
                    }
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public int InsToken(string userId, string token, DateTime date)
        {
            int response = 0;
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            int reader = 0;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Users_Access_Ins)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(1) });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                command.Parameters.Add(new SqlParameter("@Token", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(token) });
                command.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime) { SqlValue = TypeHelper.ValidateDateTime(date) });
                response = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return response;
        }

        public User GetUserById(string userId)
        {
            User response = null;
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Users_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        response = new User();
                        response.Id = reader.GetString("Id");
                        response.FirstName = reader.GetString("FirstName");
                        response.LastName = reader.GetString("LastName");
                        response.JoinDate = reader.GetDateTime("JoinDate");
                        response.Email = reader.GetString("Email");
                        response.EmailConfirmed = reader.GetBoolean("EmailConfirmed");
                        response.PhoneNumber = reader.GetString("PhoneNumber");
                        response.UserName = reader.GetString("UserName");
                    }
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public SecurityUser GetUserXId(string userId)
        {
            SecurityUser response = null;
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Users_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        response = new SecurityUser();
                        response.Id = reader.GetString("Id");
                        response.FirstName = reader.GetString("FirstName");
                        response.LastName = reader.GetString("LastName");
                        response.JoinDate = reader.GetDateTime("JoinDate");
                        response.Email = reader.GetString("Email");
                        response.EmailConfirmed = reader.GetBoolean("EmailConfirmed");
                        response.PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))? null : reader.GetString(reader.GetOrdinal("PhoneNumber"));
                        response.PhoneExtension = reader.IsDBNull(reader.GetOrdinal("PhoneExtension")) ? null : reader.GetString(reader.GetOrdinal("PhoneExtension"));
                        response.UserName = reader.GetString("UserName");
                        response.PositionId = reader.GetInt32("PositionId");
                        response.NoEmployee = reader.GetString("NoEmployee");
                        response.PhotoURL = UserStorageResolver.ResolveUrl(reader.GetString("PhotoURL"));
                        response.Position = reader.GetString("Position");
                    }

                    response.CBUs = GetUserAccesibility(response.Id);
                    response.Roles = GetRolesXId(response.Id);
                }
                reader.Close();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }




            return response;
        }

        public List<Role> GetRolesXId(string userId)
        {
            List<Role> response = new List<Role>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_RolesByUserId_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Role item = new Role();
                        item.Id = reader.GetString("Id");
                        item.Name = reader.GetString("Name");
                        item.RoleId = reader.GetInt32("RoleId");
                        item.Description = reader.IsDBNull(reader.GetOrdinal("Description"))? null: reader.GetString(reader.GetOrdinal("Description"));
                        item.Type = reader.GetString("Type");
                        response.Add(item);
                    }
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public List<Role> GetRoles()
        {
            List<Role> response = new List<Role>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Role_GETL)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Role item = new Role();
                        item.Id = reader.GetString("Id");
                        item.Name = reader.GetString("Name");
                        item.RoleId = reader.GetInt32("RoleId");
                        item.DefaultUrl = reader.IsDBNull(reader.GetOrdinal("DefaultUrl")) ? null  : reader.GetString(reader.GetOrdinal("DefaultUrl"));
                        item.UsersCount = reader.GetInt32("UsersCount");
                        item.Type = reader.GetString("Type");
                        item.Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")); 
                        response.Add(item);
                    }
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public List<Position> GetPositions()
        {
            List<Position> response = new List<Position>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Position)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(1) });

                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Position item = new Position();
                        item.Id = reader.GetInt32("Id");
                        item.Name = reader.GetString("Name");
                        item.EstatusId = reader.GetBoolean("EstatusId");
                        response.Add(item);
                    }
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public List<User> GetUsers(int Page, int PageLenght)
        {
            List<User> response = new List<User>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;


            SqlCommand command2 = null;
            SqlDataReader newReader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Users_GETL)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };



                reader = command.ExecuteReader();

                DataTableReader dataReader;
                var dataTable = new System.Data.DataTable();


                var dt = new System.Data.DataTable();
                if (reader.HasRows)
                {
                    dataTable.Load(reader);
                    dataReader = dataTable.CreateDataReader();
                    dt = new System.Data.DataTable();
                    dt.Load(dataReader);

                }
                reader.Close();


                command2 = new SqlCommand(DataObjects.Usp_Security_Users_GETL)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command2.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(1) });
                command2.Parameters.Add(new SqlParameter("@Page", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(Page) });
                command2.Parameters.Add(new SqlParameter("@PageLength", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(PageLenght) });
                newReader = command2.ExecuteReader();

                DataTable data = new DataTable();
                data.Load(newReader);
                newReader.Close();

                //if (newReader.Read())
                //{
                //    if (newReader.HasRows)
                //    {
                //        while (newReader.Read())
                //        {
                //            User item = new User();
                //            item.Id = newReader.GetString("Id");
                //            item.FirstName = newReader.GetString("FirstName");
                //            item.LastName = newReader.GetString("LastName");
                //            item.JoinDate = newReader.GetDateTime("JoinDate");
                //            item.Email = newReader.GetString("Email");
                //            item.EmailConfirmed = newReader.GetBoolean("EmailConfirmed");
                //            item.PhoneNumber = newReader.GetString("PhoneNumber");
                //            item.UserName = newReader.GetString("UserName");
                //            item.NoEmployee = newReader.GetString("NoEmployee");
                //            item.PhoneExtension = newReader.GetString("PhoneExtension");
                //            item.photoURL = newReader.GetString("PhotoURL");
                //            item.EstatusId = newReader.GetBoolean("LockoutEnabled");

                //            IEnumerable<DataRow> ieRegistro = from fila in dt.AsEnumerable()
                //                                              where fila.Field<string>("id").Equals(item.Id.ToString())
                //                                              select fila;
                //            string sRoles = "";
                //            foreach (DataRow row in ieRegistro)
                //            {
                //                sRoles = sRoles + row["RoleName"].ToString() + ", ";
                //            }
                //            if (sRoles.Length > 2)
                //            {
                //                sRoles = sRoles.Substring(0, sRoles.Length - 2);
                //            }
                //            item.Roles = sRoles;
                //            response.Add(item);
                //        }
                //    }
                //}

                foreach (DataRow row in data.Rows)
                {
                    User item = new User();
                    item.Id = row.Field<string>("Id") ?? "";
                    item.FirstName = row.Field<string>("FirstName") ?? "";
                    item.LastName = row.Field<string>("LastName") ?? "";
                    item.JoinDate = row.Field<DateTime?>("JoinDate") ?? DateTime.MinValue;
                    item.Email = row.Field<string>("Email") ?? "";
                    item.EmailConfirmed = row.Field<bool?>("EmailConfirmed") ?? false;
                    item.PhoneNumber = row.Field<string>("PhoneNumber") ?? "";
                    item.UserName = row.Field<string>("UserName") ?? "";
                    item.NoEmployee = row.Field<string>("NoEmployee") ?? "";
                    item.PhoneExtension = row.Field<string>("PhoneExtension") ?? "";
                    item.photoURL = UserStorageResolver.ResolveUrl(row.Field<string>("PhotoURL") ?? "");
                    item.EstatusId = row.Field<bool?>("LockoutEnabled") ?? false;

                    IEnumerable<DataRow> ieRegistro = from fila in dt.AsEnumerable()
                                                      where fila.Field<string>("id") == item.Id
                                                      select fila;

                    string sRoles = string.Join(", ", ieRegistro.Select(r => r["RoleName"].ToString()));
                    item.Roles = sRoles;

                    response.Add(item);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (command2 != null)
                {
                    command2.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();

                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }

                    if (newReader != null)
                    {
                        if (!newReader.IsClosed)
                        {
                            newReader.Close();
                        }
                    }
                }
            }

            return response;
        }

        public List<CBU> GetUserAccesibility(string userId)
        {
            List<CBU> response = new List<CBU>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Permissons)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();

                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                if (tablaDatos.Rows.Count > 0)
                {

                    var gruposCBU = from row in tablaDatos.AsEnumerable()
                                    group row by new
                                    {
                                        CBUID = row.Field<Guid>("CBUID"),
                                        CBUNAME = row.Field<string>("CBUNAME")
                                    } into grp
                                    select new
                                    {
                                        CBUID = grp.Key.CBUID,
                                        CBUNAME = grp.Key.CBUNAME,
                                        Conteo = grp.Count()
                                    };
                    foreach (var grupoCBU in gruposCBU)
                    {
                        CBU cBU = new CBU();
                        cBU.Id = grupoCBU.CBUID.ToString();
                        cBU.CBUName = grupoCBU.CBUNAME;

                        var gruposRegion = from row in tablaDatos.AsEnumerable()
                                           where row.Field<Guid>("CBUId") == grupoCBU.CBUID
                                           group row by new
                                           {
                                               REGIONID = row.Field<Guid>("REGIONID"),
                                               REGIONNAME = row.Field<string>("REGIONNAME")
                                           } into grp
                                           select new
                                           {
                                               REGIONID = grp.Key.REGIONID,
                                               REGIONNAME = grp.Key.REGIONNAME,
                                               Conteo = grp.Count()
                                           };
                        List<Region> regions = new List<Region>();

                        foreach (var grupoRegion in gruposRegion)
                        {

                            Region region = new Region();

                            region.RegionId = grupoRegion.REGIONID.ToString();
                            region.RegionName = grupoRegion.REGIONNAME;
                            var gruposSites = from row in tablaDatos.AsEnumerable()
                                              where row.Field<Guid>("RegionId") == grupoRegion.REGIONID
                                              group row by new
                                              {
                                                  SITIOID = row.Field<Guid>("SITIOID"),
                                                  CEDISNAME = row.Field<string>("CEDISNAME")
                                              } into grp
                                              select new
                                              {
                                                  SITIOID = grp.Key.SITIOID,
                                                  CEDISNAME = grp.Key.CEDISNAME,
                                                  Conteo = grp.Count()
                                              };
                            List<Site> sites = new List<Site>();

                            foreach (var grupoSitio in gruposSites)
                            {
                                Site site = new Site();

                                site.Id = grupoSitio.SITIOID.ToString();
                                site.CedisName = grupoSitio.CEDISNAME;
                                sites.Add(site);
                            }
                            region.Sites = sites;
                            regions.Add(region);
                        }
                        cBU.regions = regions;
                        response.Add(cBU);
                    }
                }
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public List<CBU> GetAccesibility()
        {
            List<CBU> response = new List<CBU>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                command = new SqlCommand(DataObjects.Usp_Security_Accessibility_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateInt(1) });
                reader = command.ExecuteReader();

                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                if (tablaDatos.Rows.Count > 0)
                {
                    var gruposCBU = from row in tablaDatos.AsEnumerable()
                                    group row by new
                                    {
                                        CBUID = row.Field<Guid>("CBUId"),
                                        CBUNAME = row.Field<string>("CbuName")
                                    } into grp
                                    select new
                                    {
                                        CBUID = grp.Key.CBUID,
                                        CBUNAME = grp.Key.CBUNAME,
                                        Conteo = grp.Count()
                                    };

                    foreach (var grupoCBU in gruposCBU)
                    {
                        CBU cbu = new CBU
                        {
                            Id = grupoCBU.CBUID.ToString(),
                            CBUName = grupoCBU.CBUNAME
                        };

                        var gruposRegion = from row in tablaDatos.AsEnumerable()
                                           where row.Field<Guid>("CBUId") == grupoCBU.CBUID
                                           group row by new
                                           {
                                               REGIONID = row.Field<Guid>("RegionId"),
                                               REGIONNAME = row.Field<string>("RegionName")
                                           } into grp
                                           select new
                                           {
                                               REGIONID = grp.Key.REGIONID,
                                               REGIONNAME = grp.Key.REGIONNAME,
                                               Conteo = grp.Count()
                                           };

                        List<Region> regions = new List<Region>();

                        foreach (var grupoRegion in gruposRegion)
                        {
                            Region region = new Region
                            {
                                RegionId = grupoRegion.REGIONID.ToString(),
                                RegionName = grupoRegion.REGIONNAME
                            };

                            var gruposSites = from row in tablaDatos.AsEnumerable()
                                              where row.Field<Guid>("RegionId") == grupoRegion.REGIONID
                                                    && row.Field<Guid>("CBUId") == grupoCBU.CBUID
                                              group row by new
                                              {
                                                  ID = row.Field<Guid>("Id"), // SiteByRegionId
                                                  SITIOID = row.Field<Guid>("SitioId"),
                                                  CEDISNAME = row.Field<string>("CedisName")
                                              } into grp
                                              select new
                                              {
                                                  ID = grp.Key.ID,
                                                  SITIOID = grp.Key.SITIOID,
                                                  CEDISNAME = grp.Key.CEDISNAME,
                                                  Conteo = grp.Count()
                                              };

                            List<Site> sites = new List<Site>();

                            foreach (var grupoSitio in gruposSites)
                            {
                                Site site = new Site
                                {
                                    SiteByRegionId = grupoSitio.ID.ToString(),
                                    Id = grupoSitio.SITIOID.ToString(),
                                    CedisName = grupoSitio.CEDISNAME
                                };
                                sites.Add(site);
                            }

                            region.Sites = sites;
                            regions.Add(region);
                        }

                        cbu.regions = regions;
                        response.Add(cbu);
                    }
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }
            return response;
        }

        public List<Modulos> GetUserModulo(string userId)
        {
            List<Modulos> response = new List<Modulos>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Modulo_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(1) });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();

                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                IEnumerable<DataRow> enumerableRowsTodos = tablaDatos.AsEnumerable();

                var modulosPadre = from row in enumerableRowsTodos
                                   where row.Field<int?>("IdParentModulo") == null
                                   select row;
                List<Modulos> listModulo = new List<Modulos>();

                foreach (DataRow row in modulosPadre)
                {
                    Modulos item = new Modulos();
                    item.Idmodulo = row.Field<int>("IdModulo");
                    item.IdparentModulo = row.Field<int?>("IdParentModulo") == null ? 0 : row.Field<int>("IdParentModulo");
                    item.TitleModulo = row.Field<string>("TitleModulo");
                    item.DescriptionModulo = row.Field<string>("DescriptionModulo");
                    item.OrderModulo = row.Field<int>("OrderModulo");
                    item.URLModulo = row.Field<string>("URLModulo");
                    item.Icon = row.Field<string>("Icon");
                    item.granted = true;

                    List<Modulos> listSubModulo = new List<Modulos>();
                    listSubModulo = BuildModulos(enumerableRowsTodos, item.Idmodulo);
                    item.subModulo = listSubModulo;
                    listModulo.Add(item);
                }

                response = listModulo;
                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        private List<Modulos> BuildModulos(IEnumerable<DataRow> modulosChild, int? IdPadre)
        {
            List<Modulos> listSubModulos = new List<Modulos>();

            foreach (DataRow rowhijo in modulosChild)
            {
                Modulos itemChild = new Modulos();

                itemChild.Idmodulo = rowhijo.Field<int>("IdModulo");
                itemChild.IdparentModulo = rowhijo.Field<int?>("IdParentModulo") == null ? 0 : rowhijo.Field<int>("IdParentModulo");
                itemChild.TitleModulo = rowhijo.Field<string>("TitleModulo");
                itemChild.DescriptionModulo = rowhijo.Field<string>("DescriptionModulo");
                itemChild.OrderModulo = rowhijo.Field<int>("OrderModulo");
                itemChild.URLModulo = rowhijo.Field<string>("URLModulo");
                itemChild.granted = true;

                if (itemChild.IdparentModulo.Equals(IdPadre))
                {
                    itemChild.subModulo = BuildModulos(modulosChild, itemChild.Idmodulo);
                    listSubModulos.Add(itemChild);
                }
            }
            return listSubModulos;
        }

        public ResponseBaseDto InsSecurityPermissons(Accessibility accessibility, string userId)
        {
            ResponseBaseDto response = new ResponseBaseDto();
            DataTable dt = new DataTable();
            dt.Columns.Add("userId", typeof(string));
            dt.Columns.Add("SiteByRegion", typeof(string));

            dt.Columns.Add("SiteId", typeof(string));
            dt.Columns.Add("RegionId", typeof(string));
            dt.Columns.Add("CBU", typeof(string));


            if (accessibility.CBU.Count > 0)
            {
                foreach (CBU CBU in accessibility.CBU)
                {
                    foreach (Region region in CBU.regions)
                    {
                        foreach (Site site in region.Sites)
                        {
                            DataRow row = dt.NewRow();
                            row["userId"] = userId;
                            row["SiteByRegion"] = site.SiteByRegionId.ToString();
                            row["SiteId"] = site.Id.ToString();
                            row["RegionId"] = region.RegionId.ToString();
                            row["CBU"] = CBU.Id.ToString();
                            dt.Rows.Add(row);
                        }
                    }
                }

            }
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();
                command = new SqlCommand
                {
                    CommandText = DataObjects.Usp_Security_AccessibilityNetUsers_INS,
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                command.Parameters.Add(new SqlParameter("@Accesibility", SqlDbType.Structured) { SqlValue = dt });
                var responseInsert = command.ExecuteNonQuery();
                response.Success = true;
                return response;
            }
            catch (Exception exception)
            {
                response.Code = exception.HResult.ToString();
                response.Success = false;
                response.Message = exception.Message;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return response;
        }

        public ResponseBaseDto UpdSecurityAccesibility(Accessibility accessibility, string userId)
        {
            ResponseBaseDto response = new ResponseBaseDto();
            DataTable dt = new DataTable();
            dt.Columns.Add("userId", typeof(string));
            dt.Columns.Add("SiteByRegion", typeof(string));

            dt.Columns.Add("Site", typeof(string));
            dt.Columns.Add("Region", typeof(string));
            dt.Columns.Add("CBU", typeof(string));

            if (accessibility.CBU.Count > 0)
            {
                foreach (CBU CBU in accessibility.CBU)
                {
                    foreach (Region region in CBU.regions)
                    {
                        foreach (Site site in region.Sites)
                        {
                            DataRow row = dt.NewRow();
                            row["userId"] = userId;
                            row["SiteByRegion"] = site.SiteByRegionId.ToString();
                            row["Site"] = site.Id.ToString();
                            row["Region"] = region.RegionId.ToString();
                            row["CBU"] = CBU.Id.ToString();
                            dt.Rows.Add(row);
                        }
                    }
                }

            }
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();
                command = new SqlCommand
                {
                    CommandText = DataObjects.Usp_Security_AccessibilityNetUsers_Upd,
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                command.Parameters.Add(new SqlParameter("@Accesibility", SqlDbType.Structured) { SqlValue = dt });
                var responseInsert = command.ExecuteNonQuery();
                response.Success = true;
                response.Message = "Succefully";
                return response;
            }
            catch (Exception exception)
            {
                response.Code = exception.HResult.ToString();
                response.Success = false;
                response.Message = exception.Message;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return response;
        }

        public ResponseBaseDto UpdSecurityUserRols(string userId, Role role)
        {
            ResponseBaseDto response = new ResponseBaseDto();





            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;



            try
            {
                connection.Open();
                command = new SqlCommand
                {
                    CommandText = DataObjects.Usp_Security_UpdateUserRols,
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = userId });
                command.Parameters.Add(new SqlParameter("@RolId", SqlDbType.VarChar) { SqlValue = role.Id });
                var responseInsert = command.ExecuteNonQuery();
                response.Success = true;
                response.Message = "Succefully";
                return response;


            }
            catch (Exception exception)
            {
                response.Code = exception.HResult.ToString();
                response.Success = false;
                response.Message = exception.Message;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }





            return response;
        }

        public List<Component> GetComponentById(string userId)
        {
            List<Component> response = new List<Component>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Components_By_User_Get)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(1) });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();

                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                if (tablaDatos.Rows.Count > 0)
                {
                    foreach (DataRow row in tablaDatos.Rows)
                    {
                        Component component = new Component();
                        component.IdComponent = row.Field<string>("IdComponent");
                        component.IdComponentParent = row.Field<string>("IdComponentParent");
                        component.IdTipoComponent = row.Field<int?>("IdTipoComponent");
                        component.Description = row.Field<string>("Description");
                        component.Status = row.Field<int?>("Status");
                        component.IdPermisson = row.Field<int>("IdPermisson");
                        response.Add(component);
                    }
                }


                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public List<Component> GetComponentByUser(RequestComponent request, string UserId)
        {

            List<ComponentRequest> listaComponents = request.Components;

            DataTable tablaComponentes = listaComponents.AsDataTable();

            List<Component> response = new List<Component>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Components_Get)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@Components", SqlDbType.Structured) { SqlValue = tablaComponentes });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(UserId) });
                reader = command.ExecuteReader();

                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                if (tablaDatos.Rows.Count > 0)
                {
                    foreach (DataRow row in tablaDatos.Rows)
                    {
                        Component component = new Component();
                        component.IdComponent = row.Field<string>("IdComponent");
                        component.IdComponentParent = row.Field<string>("IdComponentParent");
                        component.IdTipoComponent = row.Field<int?>("IdTipoComponent");
                        component.Description = row.Field<string>("Description");
                        component.Status = row.Field<int?>("Status");
                        component.IdPermisson = row.Field<int?>("IdPermisson");
                        response.Add(component);
                    }
                }


                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public List<Module> GetModules()
        {
            List<Module> response = new List<Module>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Module_Get)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                reader = command.ExecuteReader();

                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                IEnumerable<DataRow> enumerableRowsTodos = tablaDatos.AsEnumerable();


                foreach (DataRow row in tablaDatos.Rows)
                {
                    Module module = new Module();
                    module.Idmodulo = row.Field<int>("IdModulo");
                    module.TitleModulo = row.Field<string>("TitleModulo");
                    response.Add(module);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public ResponseBaseDto BuildPermissonRols(ModuleRequest moduleRequest)
        {
            ResponseBaseDto response = new ResponseBaseDto();
            DataTable dt = new DataTable();
            dt.Columns.Add("userId", typeof(string));
            dt.Columns.Add("SiteByRegion", typeof(string));


            DataTable dtPermisson = new DataTable();
            dtPermisson.Columns.Add("IdRol", typeof(string));
            dtPermisson.Columns.Add("IdPermisson", typeof(int));
            dtPermisson.Columns.Add("IdModulo", typeof(int));

            if (moduleRequest.SubModules.Count > 0)
            {
                foreach (var submodule in moduleRequest.SubModules)
                {
                    foreach (Permissions permissons in submodule.Permissions)
                    {
                        if (permissons.PermissionRead)
                        {
                            DataRow row = dtPermisson.NewRow();
                            row["IdRol"] = moduleRequest.ModuleIdentifier;
                            row["IdPermisson"] = 1;
                            row["IdModulo"] = permissons.PermissionIdentifier;
                            dtPermisson.Rows.Add(row);
                        }

                        if (permissons.PermissonEdit)
                        {
                            DataRow row = dtPermisson.NewRow();
                            row["IdRol"] = moduleRequest.ModuleIdentifier;
                            row["IdPermisson"] = 2;
                            row["IdModulo"] = permissons.PermissionIdentifier;
                            dtPermisson.Rows.Add(row);
                        }

                        if (permissons.PermissionCreate)
                        {
                            DataRow row = dtPermisson.NewRow();
                            row["IdRol"] = moduleRequest.ModuleIdentifier;
                            row["IdPermisson"] = 3;
                            row["IdModulo"] = permissons.PermissionIdentifier;
                            dtPermisson.Rows.Add(row);
                        }

                        if (permissons.PermissionRequest)
                        {
                            DataRow row = dtPermisson.NewRow();
                            row["IdRol"] = moduleRequest.ModuleIdentifier;
                            row["IdPermisson"] = 4;
                            row["IdModulo"] = permissons.PermissionIdentifier;
                            dtPermisson.Rows.Add(row);
                        }

                        if (permissons.PermissionAuth)
                        {
                            DataRow row = dtPermisson.NewRow();
                            row["IdRol"] = moduleRequest.ModuleIdentifier;
                            row["IdPermisson"] = 5;
                            row["IdModulo"] = permissons.PermissionIdentifier;
                            dtPermisson.Rows.Add(row);
                        }
                    }
                }
            }




            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();
                command = new SqlCommand
                {
                    CommandText = DataObjects.Usp_Security_PermissonINS,
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                command.Parameters.Add(new SqlParameter("@PermissonRoles", SqlDbType.Structured) { SqlValue = dtPermisson });
                var responseInsert = command.ExecuteNonQuery();
                response.Success = true;
                response.Message = "Succefully";
                return response;
            }
            catch (Exception exception)
            {
                response.Code = exception.HResult.ToString();
                response.Success = false;
                response.Message = exception.Message;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return response;
        }

        public ModulePermissonResponse GetModulePermisson(int idParentModulo)
        {


            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_SecurityPermissonPivot2)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@IdParentModulo", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(idParentModulo) });
                reader = command.ExecuteReader();

                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                //  IEnumerable<DataRow> enumerableRowsTodos = tablaDatos.AsEnumerable();

                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (DataRow row in tablaDatos.Rows)
                {

                    ModulePermisson modulePermisson = new ModulePermisson();
                    modulePermisson.Idmodulo = row.Field<int>("IdModulo");
                    modulePermisson.TitleModulo = row.Field<string>("TitleModulo");
                    modulePermisson.Ver = row.Field<int?>("Ver").HasValue ? 0 : 1;
                    modulePermisson.Autorizar = row.Field<int?>("Autorizar").HasValue ? 0 : 1;
                    modulePermisson.Crear = row.Field<int?>("Crear").HasValue ? 0 : 1;
                    modulePermisson.Editar = row.Field<int?>("Editar").HasValue ? 0 : 1;
                    modulePermisson.Solicitar = row.Field<int?>("Solicitar").HasValue ? 0 : 1;






                    SqlCommand command2 = null;
                    SqlDataReader reader2 = null;
                    command2 = new SqlCommand(DataObjects.Usp_SecurityPermissonPivot2)
                    {
                        Connection = connection,
                        CommandType = CommandType.StoredProcedure
                    };

                    command2.Parameters.Add(new SqlParameter("@IdParentModulo", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(row.Field<int>("IdModulo")) });
                    reader2 = command2.ExecuteReader();

                    DataTable tablaDatos2 = new DataTable();

                    tablaDatos2.Load(reader2);

                    if (tablaDatos2.Rows.Count > 0)
                    {
                        List<ModulePermisson> modulePermissonsList2 = new List<ModulePermisson>();
                        foreach (DataRow row2 in tablaDatos2.Rows)
                        {
                            ModulePermisson modulePermisson2 = new ModulePermisson();
                            modulePermisson2.Idmodulo = row2.Field<int>("IdModulo");
                            modulePermisson2.TitleModulo = row2.Field<string>("TitleModulo");
                            modulePermisson2.Ver = row2.Field<int?>("Ver").HasValue ? 0 : 1;
                            modulePermisson2.Autorizar = row2.Field<int?>("Autorizar").HasValue ? 0 : 1;
                            modulePermisson2.Crear = row2.Field<int?>("Crear").HasValue ? 0 : 1;
                            modulePermisson2.Editar = row2.Field<int?>("Editar").HasValue ? 0 : 1;
                            modulePermisson2.Solicitar = row2.Field<int?>("Solicitar").HasValue ? 0 : 1;
                            modulePermissonsList2.Add(modulePermisson2);

                        }
                        modulePermisson.modulePermissons = modulePermissonsList2;

                        reader2.Close();
                        command2.Dispose();
                    }

                    modulePermissonsList.Add(modulePermisson);

                }

                response.modules = modulePermissonsList;


                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public List<RolesAccionAlta> GetAllModulesNew()
        {
            List<RolesAccionAlta> ListRolesAccionAlta = new List<RolesAccionAlta>();

            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_security_GetModuleNew)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(1) });
                command.Parameters.Add(new SqlParameter("@TypeModule", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(0) });
                reader = command.ExecuteReader();


                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                reader.Close();
                command.Dispose();
                //RolesAccionAlta
                DataRow[] filteredRowsModulo = tablaDatos.Select("IdTypeModulo = 100");

                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (DataRow rowModulo in filteredRowsModulo)
                {

                    RolesAccionAlta rolesAccionAlta = new RolesAccionAlta();
                    rolesAccionAlta.IdModule = rowModulo.Field<int>("IdModulo");
                    rolesAccionAlta.NameModulo = rowModulo.Field<string>("NombreModulo");



                    DataRow[] filteredRowsSubmdulos = tablaDatos.Select("IdTypeModulo = 200 AND IdParentModulo = " + rowModulo.Field<int>("IdModulo").ToString());
                    List<Submodule> submodulesList = new List<Submodule>();
                    foreach (DataRow rowSubmodulo in filteredRowsSubmdulos)
                    {
                        Submodule submodule = new Submodule();
                        submodule.NameSubmodulo = rowSubmodulo.Field<string>("NombreModulo");

                        DataRow[] filteredRowsView = tablaDatos.Select("IdTypeModulo = 300 AND IdParentModulo = " + rowSubmodulo.Field<int>("IdModulo").ToString());
                        List<Views> viewList = new List<Views>();
                        foreach (DataRow rowView in filteredRowsView)
                        {
                            Views views = new Views();
                            views.ViewName = rowView.Field<string>("NombreModulo");

                            DataRow[] filteredRowsAccions = tablaDatos.Select("IdTypeModulo = 400 AND IdParentModulo = " + rowView.Field<int>("IdModulo").ToString());
                            List<ActionItem> actionsList = new List<ActionItem>();
                            foreach (DataRow rowAction in filteredRowsAccions)
                            {
                                ActionItem actions = new ActionItem();
                                actions.Name = rowAction.Field<string>("NombreModulo");
                                actions.IdAction = rowAction.Field<int>("IdModulo");
                                actions.Permisson = false;
                                actionsList.Add(actions);
                            }
                            views.Acciones = actionsList;
                            viewList.Add(views);

                        }
                        submodule.Vistas = viewList;
                        submodulesList.Add(submodule);
                    }
                    rolesAccionAlta.Modules = submodulesList;
                    ListRolesAccionAlta.Add(rolesAccionAlta);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return ListRolesAccionAlta;
        }

        public List<RolesAccionAlta> GetAllModulesByRoleNew(string RoleId)
        {
            List<RolesAccionAlta> ListRolesAccionAlta = new List<RolesAccionAlta>();

            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_security_GetModuleNew)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(2) });
                command.Parameters.Add(new SqlParameter("@TypeModule", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(0) });
                command.Parameters.Add(new SqlParameter("@RolId", SqlDbType.NVarChar) { SqlValue = TypeHelper.ValidateString(RoleId) });
                reader = command.ExecuteReader();


                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                reader.Close();
                command.Dispose();
                //RolesAccionAlta
                DataRow[] filteredRowsModulo = tablaDatos.Select("IdTypeModulo = 100");

                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (DataRow rowModulo in filteredRowsModulo)
                {

                    RolesAccionAlta rolesAccionAlta = new RolesAccionAlta();
                    rolesAccionAlta.IdModule = rowModulo.Field<int>("IdModulo");
                    rolesAccionAlta.NameModulo = rowModulo.Field<string>("NombreModulo");



                    DataRow[] filteredRowsSubmdulos = tablaDatos.Select("IdTypeModulo = 200 AND IdParentModulo = " + rowModulo.Field<int>("IdModulo").ToString());
                    List<Submodule> submodulesList = new List<Submodule>();
                    foreach (DataRow rowSubmodulo in filteredRowsSubmdulos)
                    {
                        Submodule submodule = new Submodule();
                        submodule.NameSubmodulo = rowSubmodulo.Field<string>("NombreModulo");

                        DataRow[] filteredRowsView = tablaDatos.Select("IdTypeModulo = 300 AND IdParentModulo = " + rowSubmodulo.Field<int>("IdModulo").ToString());
                        List<Views> viewList = new List<Views>();
                        foreach (DataRow rowView in filteredRowsView)
                        {
                            Views views = new Views();
                            views.ViewName = rowView.Field<string>("NombreModulo");

                            DataRow[] filteredRowsAccions = tablaDatos.Select("IdTypeModulo = 400 AND IdParentModulo = " + rowView.Field<int>("IdModulo").ToString());
                            List<ActionItem> actionsList = new List<ActionItem>();
                            foreach (DataRow rowAction in filteredRowsAccions)
                            {
                                ActionItem actions = new ActionItem();
                                actions.Name = rowAction.Field<string>("NombreModulo");
                                actions.IdAction = rowAction.Field<int>("IdModulo");
                                if (actions.IdAction.Equals(28))
                                {
                                    var test = 1;
                                }
                                var dato = rowAction.Field<bool>("Status");
                                actions.Permisson = dato;
                                actionsList.Add(actions);
                            }
                            views.Acciones = actionsList;
                            viewList.Add(views);

                        }
                        submodule.Vistas = viewList;
                        submodulesList.Add(submodule);
                    }
                    rolesAccionAlta.Modules = submodulesList;
                    ListRolesAccionAlta.Add(rolesAccionAlta);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return ListRolesAccionAlta;
        }
        public List<RolesAccionAlta> GetAllModulesByRoleNewAlong(string RoleId)
        {
            List<RolesAccionAlta> ListRolesAccionAlta = new List<RolesAccionAlta>();

            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_security_GetModuleNew)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(3) });
                command.Parameters.Add(new SqlParameter("@TypeModule", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(0) });
                command.Parameters.Add(new SqlParameter("@RolId", SqlDbType.NVarChar) { SqlValue = TypeHelper.ValidateString(RoleId) });
                reader = command.ExecuteReader();


                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                reader.Close();
                command.Dispose();
                //RolesAccionAlta
                DataRow[] filteredRowsModulo = tablaDatos.Select("IdTypeModulo = 100");

                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (DataRow rowModulo in filteredRowsModulo)
                {

                    RolesAccionAlta rolesAccionAlta = new RolesAccionAlta();
                    rolesAccionAlta.IdModule = rowModulo.Field<int>("IdModulo");
                    rolesAccionAlta.NameModulo = rowModulo.Field<string>("NombreModulo");



                    DataRow[] filteredRowsSubmdulos = tablaDatos.Select("IdTypeModulo = 200 AND IdParentModulo = " + rowModulo.Field<int>("IdModulo").ToString());
                    List<Submodule> submodulesList = new List<Submodule>();
                    foreach (DataRow rowSubmodulo in filteredRowsSubmdulos)
                    {
                        Submodule submodule = new Submodule();
                        submodule.NameSubmodulo = rowSubmodulo.Field<string>("NombreModulo");

                        DataRow[] filteredRowsView = tablaDatos.Select("IdTypeModulo = 300 AND IdParentModulo = " + rowSubmodulo.Field<int>("IdModulo").ToString());
                        List<Views> viewList = new List<Views>();
                        foreach (DataRow rowView in filteredRowsView)
                        {
                            Views views = new Views();
                            views.ViewName = rowView.Field<string>("NombreModulo");

                            DataRow[] filteredRowsAccions = tablaDatos.Select("IdTypeModulo = 400 AND IdParentModulo = " + rowView.Field<int>("IdModulo").ToString());
                            List<ActionItem> actionsList = new List<ActionItem>();
                            foreach (DataRow rowAction in filteredRowsAccions)
                            {
                                ActionItem actions = new ActionItem();
                                actions.Name = rowAction.Field<string>("NombreModulo");
                                actions.IdAction = rowAction.Field<int>("IdModulo");
                                if (actions.IdAction.Equals(28))
                                {
                                    var test = 1;
                                }
                                var dato = rowAction.Field<bool>("Status");
                                actions.Permisson = dato;
                                actionsList.Add(actions);
                            }
                            views.Acciones = actionsList;
                            viewList.Add(views);

                        }
                        submodule.Vistas = viewList;
                        submodulesList.Add(submodule);
                    }
                    rolesAccionAlta.Modules = submodulesList;
                    ListRolesAccionAlta.Add(rolesAccionAlta);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return ListRolesAccionAlta;
        }

        public List<RolesAccionAlta> GetAllModulesSpecialNew(string userId)
        {
            List<RolesAccionAlta> ListRolesAccionAlta = new List<RolesAccionAlta>();

            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_security_GetModuleNew)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(10) });
                command.Parameters.Add(new SqlParameter("@TypeModule", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(0) });
                command.Parameters.Add(new SqlParameter("@RolId", SqlDbType.NVarChar) { SqlValue = TypeHelper.ValidateString("") });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();


                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                reader.Close();
                command.Dispose();
                //RolesAccionAlta
                DataRow[] filteredRowsModulo = tablaDatos.Select("IdTypeModulo = 100");

                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (DataRow rowModulo in filteredRowsModulo)
                {

                    RolesAccionAlta rolesAccionAlta = new RolesAccionAlta();
                    rolesAccionAlta.IdModule = rowModulo.Field<int>("IdModulo");
                    rolesAccionAlta.NameModulo = rowModulo.Field<string>("NombreModulo");



                    DataRow[] filteredRowsSubmdulos = tablaDatos.Select("IdTypeModulo = 200 AND IdParentModulo = " + rowModulo.Field<int>("IdModulo").ToString());
                    List<Submodule> submodulesList = new List<Submodule>();
                    foreach (DataRow rowSubmodulo in filteredRowsSubmdulos)
                    {
                        Submodule submodule = new Submodule();
                        submodule.NameSubmodulo = rowSubmodulo.Field<string>("NombreModulo");

                        DataRow[] filteredRowsView = tablaDatos.Select("IdTypeModulo = 300 AND IdParentModulo = " + rowSubmodulo.Field<int>("IdModulo").ToString());
                        List<Views> viewList = new List<Views>();
                        foreach (DataRow rowView in filteredRowsView)
                        {
                            Views views = new Views();
                            views.ViewName = rowView.Field<string>("NombreModulo");

                            DataRow[] filteredRowsAccions = tablaDatos.Select("IdTypeModulo = 400 AND IdParentModulo = " + rowView.Field<int>("IdModulo").ToString());
                            List<ActionItem> actionsList = new List<ActionItem>();
                            foreach (DataRow rowAction in filteredRowsAccions)
                            {
                                ActionItem actions = new ActionItem();
                                actions.Name = rowAction.Field<string>("NombreModulo");
                                actions.IdAction = rowAction.Field<int>("IdModulo");
                                if (actions.IdAction.Equals(28))
                                {
                                    var test = 1;
                                }
                                var dato = rowAction.Field<bool>("Status");
                                actions.Permisson = dato;
                                actionsList.Add(actions);
                            }
                            views.Acciones = actionsList;
                            viewList.Add(views);

                        }
                        submodule.Vistas = viewList;
                        submodulesList.Add(submodule);
                    }
                    rolesAccionAlta.Modules = submodulesList;
                    ListRolesAccionAlta.Add(rolesAccionAlta);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return ListRolesAccionAlta;
        }

        public ModulePermissonResponse GetModuleDetail()
        {


            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Modules_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                // command.Parameters.Add(new SqlParameter("@IdParentModulo", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(idParentModulo) });
                reader = command.ExecuteReader();
                DataTable tablaDatos = new DataTable();
                tablaDatos.Load(reader);



                var Master = (from DataRow fila in tablaDatos.Rows
                              where (int)fila["Nivel"] == 1
                              select new { Columna1 = fila["IdModulo"], Columna2 = fila["TitleModulo"] }).Distinct();


                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (var row in Master)
                {
                    ModulePermisson modulePermisson = new ModulePermisson();
                    modulePermisson.Idmodulo = int.Parse(row.Columna1.ToString());
                    modulePermisson.TitleModulo = row.Columna2.ToString();
                    modulePermisson.idVer = 1;
                    modulePermisson.Ver = 0;
                    modulePermisson.IdEditar = 2;
                    modulePermisson.Editar = 0;
                    modulePermisson.idCrear = 3;
                    modulePermisson.Crear = 0;
                    modulePermisson.idSolicitar = 4;
                    modulePermisson.Solicitar = 0;
                    modulePermisson.idAutorizar = 5;
                    modulePermisson.Autorizar = 0;
                    modulePermisson.idBorrar = 6;
                    modulePermisson.Borrar = 0;


                    var Hijos = from DataRow fila in tablaDatos.Rows
                                where (int)fila["IdModulo"] == int.Parse(row.Columna1.ToString())
                                select fila;

                    if (Hijos.Any())
                    {
                        List<ModulePermisson> modulePermissonsListHijos = new List<ModulePermisson>();
                        // Llamar al método recursivo para procesar los hijos
                        ProcesarHijosRecursivo(Hijos.ToArray(), modulePermissonsListHijos, tablaDatos);
                        modulePermisson.modulePermissons = modulePermissonsListHijos;
                    }


                    modulePermissonsList.Add(modulePermisson);

                }


                response.modules = modulePermissonsList;


                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }
        void ProcesarHijosRecursivo(DataRow[] hijos, List<ModulePermisson> listaPermisos, DataTable tablaDatos)
        {
            foreach (DataRow rowHijo in hijos)
            {
                ModulePermisson modulePermissonHijo = new ModulePermisson();
                modulePermissonHijo.Idmodulo = rowHijo.Field<int>("IdSubModulo");
                modulePermissonHijo.TitleModulo = rowHijo.Field<string>("TitleSubModulo");
                modulePermissonHijo.idVer = rowHijo.Field<int?>("Ver").HasValue ? 1 : 0;
                modulePermissonHijo.Ver = rowHijo.Field<int?>("Ver").HasValue ? 0 : 1;
                modulePermissonHijo.IdEditar = rowHijo.Field<int?>("Editar").HasValue ? 2 : 1;
                modulePermissonHijo.Editar = rowHijo.Field<int?>("Editar").HasValue ? 0 : 1;
                modulePermissonHijo.idCrear = rowHijo.Field<int?>("Crear").HasValue ? 3 : 1;
                modulePermissonHijo.Crear = rowHijo.Field<int?>("Crear").HasValue ? 0 : 1;
                modulePermissonHijo.idSolicitar = rowHijo.Field<int?>("Solicitar").HasValue ? 4 : 1;
                modulePermissonHijo.Solicitar = rowHijo.Field<int?>("Solicitar").HasValue ? 0 : 1;
                modulePermissonHijo.idAutorizar = rowHijo.Field<int?>("Autorizar").HasValue ? 5 : 0;
                modulePermissonHijo.Autorizar = rowHijo.Field<int?>("Autorizar").HasValue ? 0 : 1;
                modulePermissonHijo.idBorrar = rowHijo.Field<int?>("Autorizar").HasValue ? 6 : 0;
                modulePermissonHijo.Borrar = rowHijo.Field<int?>("Autorizar").HasValue ? 0 : 1;
                // Obtener los subhijos del módulo actual (llamada recursiva)
                var subHijos = from DataRow fila in tablaDatos.Rows
                               where (int)fila["IdModulo"] == modulePermissonHijo.Idmodulo
                               select fila;
                if (subHijos.Any())
                {
                    // Crear una nueva lista para los subhijos
                    List<ModulePermisson> subHijosPermisos = new List<ModulePermisson>();
                    // Llamar recursivamente al método para procesar los subhijos
                    ProcesarHijosRecursivo(subHijos.ToArray(), subHijosPermisos, tablaDatos);
                    // Asignar los subhijos al módulo actual
                    modulePermissonHijo.modulePermissons = subHijosPermisos;
                }
                // Agregar el módulo actual a la lista de permisos
                listaPermisos.Add(modulePermissonHijo);
            }
        }
        public ModulePermissonResponse GetModuleDetailRol(string RoleId, int View)
        {
            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlCommand commandPermisos = null;
            SqlDataReader reader = null;
            SqlDataReader readerPermisos = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Modules_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                // command.Parameters.Add(new SqlParameter("@IdParentModulo", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(idParentModulo) });
                reader = command.ExecuteReader();
                DataTable tablaDatos = new DataTable();
                tablaDatos.Load(reader);


                commandPermisos = new SqlCommand(DataObjects.Usp_Security_Rol_Get)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                commandPermisos.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = View });
                commandPermisos.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.VarChar) { SqlValue = RoleId });
                readerPermisos = commandPermisos.ExecuteReader();
                DataTable tablaDatosPermisos = new DataTable();
                tablaDatosPermisos.Load(readerPermisos);



                var Master = (from DataRow fila in tablaDatos.Rows
                              where (int)fila["Nivel"] == 1
                              select new { Columna1 = fila["IdModulo"], Columna2 = fila["TitleModulo"] }).Distinct();

                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (var row in Master)
                {
                    ModulePermisson modulePermisson = new ModulePermisson();
                    modulePermisson.Idmodulo = int.Parse(row.Columna1.ToString());
                    modulePermisson.TitleModulo = row.Columna2.ToString();




                    //Modificar permisos desde base de datos
                    modulePermisson.idVer = 1;
                    var resultados1 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 1
                                      select rowper;
                    var fila1 = resultados1.FirstOrDefault();
                    modulePermisson.Ver = fila1?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados2 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 2
                                      select rowper;
                    var fila2 = resultados2.FirstOrDefault();
                    modulePermisson.IdEditar = 2;
                    modulePermisson.Editar = fila2?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados3 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 3
                                      select rowper;
                    var fila3 = resultados3.FirstOrDefault();
                    modulePermisson.idCrear = 3;
                    modulePermisson.Crear = fila3?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados4 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 4
                                      select rowper;
                    var fila4 = resultados4.FirstOrDefault();
                    modulePermisson.idSolicitar = 4;
                    modulePermisson.Solicitar = fila4?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados5 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 5
                                      select rowper;
                    var fila5 = resultados5.FirstOrDefault();
                    modulePermisson.idAutorizar = 5;
                    modulePermisson.Autorizar = fila5?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados6 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 6
                                      select rowper;
                    var fila6 = resultados6.FirstOrDefault();
                    modulePermisson.idBorrar = 6;
                    modulePermisson.Borrar = fila6?.Field<int?>("Permisson") == null ? 0 : 1;


                    var Hijos = from DataRow fila in tablaDatos.Rows
                                where (int)fila["IdModulo"] == int.Parse(row.Columna1.ToString())
                                select fila;

                    if (Hijos.Any())
                    {
                        List<ModulePermisson> modulePermissonsListHijos = new List<ModulePermisson>();
                        // Llamar al método recursivo para procesar los hijos
                        ProcesarHijosRecursivo2(Hijos.ToArray(), modulePermissonsListHijos, tablaDatos, tablaDatosPermisos);
                        modulePermisson.modulePermissons = modulePermissonsListHijos;
                    }


                    modulePermissonsList.Add(modulePermisson);

                }


                response.modules = modulePermissonsList;


                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }
        void ProcesarHijosRecursivo2(DataRow[] hijos, List<ModulePermisson> listaPermisos, DataTable tablaDatos, DataTable tablaDatosPermisos)
        {
            foreach (DataRow rowHijo in hijos)
            {
                ModulePermisson modulePermissonHijo = new ModulePermisson();
                modulePermissonHijo.Idmodulo = rowHijo.Field<int>("IdSubModulo");
                modulePermissonHijo.TitleModulo = rowHijo.Field<string>("TitleSubModulo");

                if (rowHijo[1].ToString().Equals("Configuracion"))
                {
                    int i = 1;
                }

                var resultados1 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  && rowper.Field<int>("IdPermisson") == 1
                                  select rowper;
                var fila1 = resultados1.FirstOrDefault();
                modulePermissonHijo.idVer = rowHijo.Field<int?>("Ver").HasValue ? 1 : 0;
                modulePermissonHijo.Ver = fila1?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados2 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  && rowper.Field<int>("IdPermisson") == 2
                                  select rowper;
                var fila2 = resultados2.FirstOrDefault();
                modulePermissonHijo.IdEditar = rowHijo.Field<int?>("Editar").HasValue ? 2 : 1;
                modulePermissonHijo.Editar = fila2?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados3 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  && rowper.Field<int>("IdPermisson") == 3
                                  select rowper;
                var fila3 = resultados3.FirstOrDefault();
                modulePermissonHijo.idCrear = rowHijo.Field<int?>("Crear").HasValue ? 3 : 1;
                modulePermissonHijo.Crear = fila3?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados4 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  && rowper.Field<int>("IdPermisson") == 4
                                  select rowper;
                var fila4 = resultados4.FirstOrDefault();
                modulePermissonHijo.idSolicitar = rowHijo.Field<int?>("Solicitar").HasValue ? 4 : 1;
                modulePermissonHijo.Solicitar = fila4?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados5 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  && rowper.Field<int>("IdPermisson") == 5
                                  select rowper;
                var fila5 = resultados5.FirstOrDefault();
                modulePermissonHijo.idAutorizar = rowHijo.Field<int?>("Autorizar").HasValue ? 5 : 0;
                modulePermissonHijo.Autorizar = fila5?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados6 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  && rowper.Field<int>("IdPermisson") == 6
                                  select rowper;
                var fila6 = resultados5.FirstOrDefault();
                modulePermissonHijo.idBorrar = rowHijo.Field<int?>("Autorizar").HasValue ? 6 : 0;
                modulePermissonHijo.Borrar = fila6?.Field<int?>("Permisson") == null ? 0 : 1;
                // Obtener los subhijos del módulo actual (llamada recursiva)
                var subHijos = from DataRow fila in tablaDatos.Rows
                               where (int)fila["IdModulo"] == modulePermissonHijo.Idmodulo
                               select fila;
                if (subHijos.Any())
                {
                    // Crear una nueva lista para los subhijos
                    List<ModulePermisson> subHijosPermisos = new List<ModulePermisson>();
                    // Llamar recursivamente al método para procesar los subhijos
                    ProcesarHijosRecursivo2(subHijos.ToArray(), subHijosPermisos, tablaDatos, tablaDatosPermisos);
                    // Asignar los subhijos al módulo actual
                    modulePermissonHijo.modulePermissons = subHijosPermisos;
                }
                // Agregar el módulo actual a la lista de permisos
                listaPermisos.Add(modulePermissonHijo);
            }
        }

        void ProcesarHijosRecursivo22(DataRow[] hijos, List<ModulePermisson2> listaPermisos, DataTable tablaDatos, DataTable tablaDatosPermisos)
        {
            foreach (DataRow rowHijo in hijos)
            {
                ModulePermisson2 modulePermissonHijo = new ModulePermisson2();
                modulePermissonHijo.Idmodulo = rowHijo.Field<int>("IdSubModulo");
                modulePermissonHijo.TitleModulo = rowHijo.Field<string>("TitleSubModulo");

                if (rowHijo[1].ToString().Equals("Configuracion"))
                {
                    int i = 1;
                }

                var resultados1 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  // && rowper.Field<int>("IdPermisson") == 1
                                  select rowper;
                var fila1 = resultados1.FirstOrDefault();
                //modulePermissonHijo.idVer = rowHijo.Field<int?>("Ver").HasValue ? 1 : 0;
                //modulePermissonHijo.Ver = fila1?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados2 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  // && rowper.Field<int>("IdPermisson") == 2
                                  select rowper;
                var fila2 = resultados2.FirstOrDefault();
                //modulePermissonHijo.IdEditar = rowHijo.Field<int?>("Editar").HasValue ? 2 : 1;
                //modulePermissonHijo.Editar = fila2?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados3 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  // && rowper.Field<int>("IdPermisson") == 3
                                  select rowper;
                var fila3 = resultados3.FirstOrDefault();
                //modulePermissonHijo.idCrear = rowHijo.Field<int?>("Crear").HasValue ? 3 : 1;
                //modulePermissonHijo.Crear = fila3?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados4 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  // && rowper.Field<int>("IdPermisson") == 4
                                  select rowper;
                var fila4 = resultados4.FirstOrDefault();
                //modulePermissonHijo.idSolicitar = rowHijo.Field<int?>("Solicitar").HasValue ? 4 : 1;
                //modulePermissonHijo.Solicitar = fila4?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados5 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  //  && rowper.Field<int>("IdPermisson") == 5
                                  select rowper;
                var fila5 = resultados5.FirstOrDefault();
                //modulePermissonHijo.idAutorizar = rowHijo.Field<int?>("Autorizar").HasValue ? 5 : 0;
                //modulePermissonHijo.Autorizar = fila5?.Field<int?>("Permisson") == null ? 0 : 1;

                var resultados6 = from rowper in tablaDatosPermisos.AsEnumerable()
                                  where rowper.Field<int>("IdSubModule") == rowHijo.Field<int>("IdSubModulo")
                                  // && rowper.Field<int>("IdPermisson") == 6
                                  select rowper;
                var fila6 = resultados5.FirstOrDefault();
                //modulePermissonHijo.idBorrar = rowHijo.Field<int?>("Autorizar").HasValue ? 6 : 0;
                //modulePermissonHijo.Borrar = fila6?.Field<int?>("Permisson") == null ? 0 : 1;
                // Obtener los subhijos del módulo actual (llamada recursiva)
                var subHijos = from DataRow fila in tablaDatos.Rows
                               where (int)fila["IdModulo"] == modulePermissonHijo.Idmodulo
                               select fila;
                if (subHijos.Any())
                {
                    // Crear una nueva lista para los subhijos
                    List<ModulePermisson2> subHijosPermisos = new List<ModulePermisson2>();
                    // Llamar recursivamente al método para procesar los subhijos
                    ProcesarHijosRecursivo22(subHijos.ToArray(), subHijosPermisos, tablaDatos, tablaDatosPermisos);
                    // Asignar los subhijos al módulo actual
                    modulePermissonHijo.modulePermissons = subHijosPermisos;
                }
                // Agregar el módulo actual a la lista de permisos
                listaPermisos.Add(modulePermissonHijo);
            }
        }
        public List<string> GetRolId(string RolName)
        {
            List<string> response = new List<string>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlDataReader reader = null;

            try
            {


                string query = "SELECT [Id], [RoleId] FROM [dbo].[AspNetRoles] WHERE Name = '" + RolName + "'";



                SqlCommand command = new SqlCommand(query, connection);
                // Asegúrate de cambiar "@valor" por el nombre real de tu parámetro y asignar el valor correctamente
                //  command.Parameters.AddWithValue("@valor", RolName);

                try
                {
                    connection.Open();
                    // Ejecuta la consulta y obtiene el resultado
                    // execute command and convert the result



                    reader = command.ExecuteReader();

                    DataTable tablaDatos = new DataTable();

                    tablaDatos.Load(reader);


                    if (tablaDatos.Rows.Count > 0)
                    {
                        response.Add(tablaDatos.Rows[0]["Id"].ToString());
                        response.Add(tablaDatos.Rows[0]["RoleId"].ToString());

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (command != null)
                {
                    command.Dispose();
                }


            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }
        public ResponseBaseDto CreateRol(PermissonRolesRequest request, List<string> strRolId)
        {
            ResponseBaseDto response = new ResponseBaseDto
            {
                Success = true,
                ErrorList = new List<ErrorDto>()
            };
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                //*****************************************************************************************************************
                try
                {
                    var requestModulos = (from n in request.Permissons
                                          select new
                                          {
                                              n.IdModulo,
                                              RoleId = strRolId[0].ToString(),
                                              RolesId = int.Parse(strRolId[1].ToString()),
                                          }).ToList();
                    var requestModulos2 = requestModulos
                                            .GroupBy(n => new { n.IdModulo, RoleId = n.RoleId.ToString(), n.RolesId })
                                            .Select(g => g.First())
                                            .Select(n => new
                                            {
                                                n.IdModulo,
                                                RoleId = n.RoleId.ToString(),
                                                RolesId = n.RolesId
                                            })
                                             .ToList();
                    DataTable requestModulesAccesos = new DataTable();
                    requestModulesAccesos.Columns.Add("IdModulo", typeof(int));
                    requestModulesAccesos.Columns.Add("RoleId", typeof(string));
                    requestModulesAccesos.Columns.Add("RolesId", typeof(int));
                    requestModulesAccesos = requestModulos2.ToList().AsDataTable();

                    using (SqlBulkCopy sbCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        sbCopy.ColumnMappings.Add("IdModulo", "ModuloId");
                        sbCopy.ColumnMappings.Add("RoleId", "RoleId");
                        sbCopy.ColumnMappings.Add("RolesId", "RolesId");
                        sbCopy.BulkCopyTimeout = 0;
                        sbCopy.BatchSize = 10000;
                        sbCopy.DestinationTableName = TableObjects.ModuloAccesos;
                        sbCopy.WriteToServer(requestModulesAccesos);
                        sbCopy.Close();
                    }
                }
                catch (Exception exception)
                {
                    response.Code = ErrorFlag.ERROR.ToString();
                    response.Success = false;
                    response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.ModuloAccesos), TechnicalMessage = exception.Message });
                }

                //***************************************************************************************************************************************************

                var requestPermissons = (from n in request.Permissons
                                         select new
                                         {
                                             RolId = strRolId[0].ToString(),
                                             n.IdModulo,
                                             n.IdPermison
                                         }).ToList();
                DataTable requestPermissonRoles = new DataTable();
                requestPermissonRoles.Columns.Add("RolId", typeof(string));
                requestPermissonRoles.Columns.Add("IdModulo", typeof(int));
                requestPermissonRoles.Columns.Add("IdPermisson", typeof(int));

                requestPermissonRoles = requestPermissons.ToList().AsDataTable();

                using (SqlBulkCopy sbCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    sbCopy.ColumnMappings.Add("RolId", "IdRol");
                    sbCopy.ColumnMappings.Add("IdPermison", "IdPermisson");
                    sbCopy.ColumnMappings.Add("IdModulo", "IdModulo");
                    sbCopy.BulkCopyTimeout = 0;
                    sbCopy.BatchSize = 10000;
                    sbCopy.DestinationTableName = TableObjects.PermissonRoles;
                    sbCopy.WriteToServer(requestPermissonRoles);
                    sbCopy.Close();
                }
                SqlCommand command2 = null;

                command2 = new SqlCommand(DataObjects.Usp_Security_Rol_UDP)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command2.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar) { SqlValue = strRolId[0] });
                command2.Parameters.Add(new SqlParameter("@RolName", SqlDbType.VarChar) { SqlValue = request.RolName });
                command2.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { SqlValue = request.Description });
                command2.Parameters.Add(new SqlParameter("@Type", SqlDbType.VarChar) { SqlValue = request.Type });
                command2.Transaction = transaction;
                command2.ExecuteScalar();


                response.Message = "Proceso completado";
                transaction.Commit();
            }
            catch (Exception exception)
            {
                response.Code = ErrorFlag.ERROR.ToString();
                response.Success = false;
                response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.PermissonRoles), TechnicalMessage = exception.Message });
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return response;
        }

        public ResponseBaseDto CreateRol2(PermissonRolesRequest2 request, List<string> strRolId)
        {
            ResponseBaseDto response = new ResponseBaseDto
            {
                Success = true,
                ErrorList = new List<ErrorDto>()
            };
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                //*****************************************************************************************************************
                try
                {
                    var requestModulos = (from n in request.Permissons
                                          select new
                                          {
                                              n.IdModulo,
                                              RoleId = strRolId[0].ToString(),
                                              RolesId = int.Parse(strRolId[1].ToString()),
                                          }).ToList();
                    var requestModulos2 = requestModulos
                                            .GroupBy(n => new { n.IdModulo, RoleId = n.RoleId.ToString(), n.RolesId })
                                            .Select(g => g.First())
                                            .Select(n => new
                                            {
                                                n.IdModulo,
                                                RoleId = n.RoleId.ToString(),
                                                RolesId = n.RolesId
                                            })
                                             .ToList();
                    DataTable requestModulesAccesos = new DataTable();
                    requestModulesAccesos.Columns.Add("IdModulo", typeof(int));
                    requestModulesAccesos.Columns.Add("RoleId", typeof(string));
                    requestModulesAccesos.Columns.Add("RolesId", typeof(int));
                    requestModulesAccesos = requestModulos2.ToList().AsDataTable();

                    using (SqlBulkCopy sbCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        sbCopy.ColumnMappings.Add("IdModulo", "ModuloId");
                        sbCopy.ColumnMappings.Add("RoleId", "RoleId");
                        sbCopy.ColumnMappings.Add("RolesId", "RolesId");
                        sbCopy.BulkCopyTimeout = 0;
                        sbCopy.BatchSize = 10000;
                        sbCopy.DestinationTableName = TableObjects.ModuloAccesos;
                        sbCopy.WriteToServer(requestModulesAccesos);
                        sbCopy.Close();
                    }
                }
                catch (Exception exception)
                {
                    response.Code = ErrorFlag.ERROR.ToString();
                    response.Success = false;
                    response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.ModuloAccesos), TechnicalMessage = exception.Message });
                }

                SqlCommand command2 = null;

                command2 = new SqlCommand(DataObjects.Usp_Security_Rol_UDP)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command2.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar) { SqlValue = strRolId[0] });
                command2.Parameters.Add(new SqlParameter("@RolName", SqlDbType.VarChar) { SqlValue = request.RolName });
                command2.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { SqlValue = request.Description });
                command2.Parameters.Add(new SqlParameter("@Type", SqlDbType.VarChar) { SqlValue = request.Type });
                command2.Transaction = transaction;
                command2.ExecuteScalar();


                response.Message = "Proceso completado";
                transaction.Commit();
            }
            catch (Exception exception)
            {
                response.Code = ErrorFlag.ERROR.ToString();
                response.Success = false;
                response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.PermissonRoles), TechnicalMessage = exception.Message });
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return response;
        }
        public List<string> GetRolIdById(string RolId)
        {
            List<string> response = new List<string>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlDataReader reader = null;

            try
            {


                string query = "SELECT [Id], [RoleId] FROM [dbo].[AspNetRoles] WHERE [Id] = '" + RolId + "'";



                SqlCommand command = new SqlCommand(query, connection);
                // Asegúrate de cambiar "@valor" por el nombre real de tu parámetro y asignar el valor correctamente
                //  command.Parameters.AddWithValue("@valor", RolName);

                try
                {
                    connection.Open();
                    // Ejecuta la consulta y obtiene el resultado
                    // execute command and convert the result



                    reader = command.ExecuteReader();

                    DataTable tablaDatos = new DataTable();

                    tablaDatos.Load(reader);


                    if (tablaDatos.Rows.Count > 0)
                    {
                        response.Add(tablaDatos.Rows[0]["Id"].ToString());
                        response.Add(tablaDatos.Rows[0]["RoleId"].ToString());

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (command != null)
                {
                    command.Dispose();
                }


            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }
        public ResponseBaseDto UpdateRol(PermissonRolesRequest request, List<string> strRolId)
        {
            ResponseBaseDto response = new ResponseBaseDto
            {
                Success = true,
                ErrorList = new List<ErrorDto>()
            };

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlCommand command2 = null;
            SqlDataReader reader = null;
            Guid guid = Guid.NewGuid();
            try
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                command = new SqlCommand(DataObjects.Usp_Security_PermissonRoles_DEL)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.VarChar) { SqlValue = strRolId[0] });
                command.Transaction = transaction;
                command.ExecuteScalar();



                command2 = new SqlCommand(DataObjects.Usp_Security_Rol_UDP)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command2.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar) { SqlValue = strRolId[0] });
                command2.Parameters.Add(new SqlParameter("@RolName", SqlDbType.VarChar) { SqlValue = request.RolName });
                command2.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { SqlValue = request.Description });
                command2.Parameters.Add(new SqlParameter("@Type", SqlDbType.VarChar) { SqlValue = request.Type });
                command2.Transaction = transaction;
                command2.ExecuteScalar();
                //*****************************************************************************************************************
                try
                {
                    var requestModulos = (from n in request.Permissons
                                          select new
                                          {
                                              n.IdModulo,
                                              RoleId = strRolId[0].ToString(),
                                              RolesId = int.Parse(strRolId[1].ToString()),
                                          }).ToList();

                    var requestModulos2 = requestModulos
                                            .GroupBy(n => new { n.IdModulo, RoleId = n.RoleId.ToString(), n.RolesId })
                                            .Select(g => g.First())
                                            .Select(n => new
                                            {
                                                n.IdModulo,
                                                RoleId = n.RoleId.ToString(),
                                                RolesId = n.RolesId
                                            })
                                             .ToList();

                    DataTable requestModulesAccesos = new DataTable();
                    requestModulesAccesos.Columns.Add("IdModulo", typeof(int));
                    requestModulesAccesos.Columns.Add("RoleId", typeof(string));
                    requestModulesAccesos.Columns.Add("RolesId", typeof(int));
                    requestModulesAccesos = requestModulos2.ToList().AsDataTable();
                    using (SqlBulkCopy sbCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        sbCopy.ColumnMappings.Add("IdModulo", "ModuloId");
                        sbCopy.ColumnMappings.Add("RoleId", "RoleId");
                        sbCopy.ColumnMappings.Add("RolesId", "RolesId");
                        sbCopy.BulkCopyTimeout = 0;
                        sbCopy.BatchSize = 10000;
                        sbCopy.DestinationTableName = TableObjects.ModuloAccesos;
                        sbCopy.WriteToServer(requestModulesAccesos);
                        sbCopy.Close();
                    }
                }
                catch (Exception exception)
                {
                    response.Code = ErrorFlag.ERROR.ToString();
                    response.Success = false;
                    response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.ModuloAccesos), TechnicalMessage = exception.Message });
                }
                //***************************************************************************************************************************************************
                //***************************************************************************************************************************************************
                //***************************************************************************************************************************************************
                var requestPermissons = (from n in request.Permissons
                                         select new
                                         {
                                             RolId = strRolId[0].ToString(),
                                             n.IdModulo,
                                             n.IdPermison
                                         }).ToList();
                DataTable requestPermissonRoles = new DataTable();
                requestPermissonRoles.Columns.Add("RolId", typeof(string));
                requestPermissonRoles.Columns.Add("IdModulo", typeof(int));
                requestPermissonRoles.Columns.Add("IdPermisson", typeof(int));
                requestPermissonRoles = requestPermissons.ToList().AsDataTable();
                using (SqlBulkCopy sbCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    sbCopy.ColumnMappings.Add("RolId", "IdRol");
                    sbCopy.ColumnMappings.Add("IdPermison", "IdPermisson");
                    sbCopy.ColumnMappings.Add("IdModulo", "IdModulo");
                    sbCopy.BulkCopyTimeout = 0;
                    sbCopy.BatchSize = 10000;
                    sbCopy.DestinationTableName = TableObjects.PermissonRoles;
                    sbCopy.WriteToServer(requestPermissonRoles);
                    sbCopy.Close();
                }
                response.Message = "Proceso completado";
                transaction.Commit();
            }
            catch (Exception exception)
            {
                response.Code = ErrorFlag.ERROR.ToString();
                response.Success = false;
                response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.PermissonRoles), TechnicalMessage = exception.Message });
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }
            return response;
        }
        public ResponseBaseDto UpdateRol2(PermissonRolesRequest2 request, List<string> strRolId)
        {
            ResponseBaseDto response = new ResponseBaseDto
            {
                Success = true,
                ErrorList = new List<ErrorDto>()
            };

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlCommand command2 = null;
            SqlDataReader reader = null;
            Guid guid = Guid.NewGuid();
            try
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                command = new SqlCommand(DataObjects.Usp_Security_PermissonRoles_DEL)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.VarChar) { SqlValue = strRolId[0] });
                command.Transaction = transaction;
                command.ExecuteScalar();



                command2 = new SqlCommand(DataObjects.Usp_Security_Rol_UDP)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command2.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar) { SqlValue = strRolId[0] });
                command2.Parameters.Add(new SqlParameter("@RolName", SqlDbType.VarChar) { SqlValue = request.RolName });
                command2.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { SqlValue = request.Description });
                command2.Parameters.Add(new SqlParameter("@Type", SqlDbType.VarChar) { SqlValue = request.Type });
                command2.Transaction = transaction;
                command2.ExecuteScalar();
                //*****************************************************************************************************************
                try
                {
                    var requestModulos = (from n in request.Permissons
                                          select new
                                          {
                                              n.IdModulo,
                                              RoleId = strRolId[0].ToString(),
                                              RolesId = int.Parse(strRolId[1].ToString()),
                                          }).ToList();

                    var requestModulos2 = requestModulos
                                            .GroupBy(n => new { n.IdModulo, RoleId = n.RoleId.ToString(), n.RolesId })
                                            .Select(g => g.First())
                                            .Select(n => new
                                            {
                                                n.IdModulo,
                                                RoleId = n.RoleId.ToString(),
                                                RolesId = n.RolesId
                                            })
                                             .ToList();

                    DataTable requestModulesAccesos = new DataTable();
                    requestModulesAccesos.Columns.Add("IdModulo", typeof(int));
                    requestModulesAccesos.Columns.Add("RoleId", typeof(string));
                    requestModulesAccesos.Columns.Add("RolesId", typeof(int));
                    requestModulesAccesos = requestModulos2.ToList().AsDataTable();
                    using (SqlBulkCopy sbCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        sbCopy.ColumnMappings.Add("IdModulo", "ModuloId");
                        sbCopy.ColumnMappings.Add("RoleId", "RoleId");
                        sbCopy.ColumnMappings.Add("RolesId", "RolesId");
                        sbCopy.BulkCopyTimeout = 0;
                        sbCopy.BatchSize = 10000;
                        sbCopy.DestinationTableName = TableObjects.ModuloAccesos;
                        sbCopy.WriteToServer(requestModulesAccesos);
                        sbCopy.Close();
                    }
                }
                catch (Exception exception)
                {
                    response.Code = ErrorFlag.ERROR.ToString();
                    response.Success = false;
                    response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.ModuloAccesos), TechnicalMessage = exception.Message });
                }
                //***************************************************************************************************************************************************
                //***************************************************************************************************************************************************
                //***************************************************************************************************************************************************
                response.Message = "Proceso completado";
                transaction.Commit();
            }
            catch (Exception exception)
            {
                response.Code = ErrorFlag.ERROR.ToString();
                response.Success = false;
                response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.PermissonRoles), TechnicalMessage = exception.Message });
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }
            return response;
        }
        public ModulePermissonResponse GetModulePermissonByUser(string userId, int view)
        {

            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlCommand commandPermisos = null;
            SqlDataReader reader = null;
            SqlDataReader readerPermisos = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Modules_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                // command.Parameters.Add(new SqlParameter("@IdParentModulo", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(idParentModulo) });
                reader = command.ExecuteReader();
                DataTable tablaDatos = new DataTable();
                tablaDatos.Load(reader);


                commandPermisos = new SqlCommand(DataObjects.Usp_Security_PermissonSpecial_Get)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                commandPermisos.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = view });
                commandPermisos.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = userId });
                readerPermisos = commandPermisos.ExecuteReader();
                DataTable tablaDatosPermisos = new DataTable();
                tablaDatosPermisos.Load(readerPermisos);



                var Master = (from DataRow fila in tablaDatos.Rows
                              where (int)fila["Nivel"] == 1
                              select new { Columna1 = fila["IdModulo"], Columna2 = fila["TitleModulo"] }).Distinct();

                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (var row in Master)
                {
                    ModulePermisson modulePermisson = new ModulePermisson();
                    modulePermisson.Idmodulo = int.Parse(row.Columna1.ToString());
                    modulePermisson.TitleModulo = row.Columna2.ToString();




                    //Modificar permisos desde base de datos
                    modulePermisson.idVer = 1;
                    var resultados1 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 1
                                      select rowper;
                    var fila1 = resultados1.FirstOrDefault();
                    modulePermisson.Ver = fila1?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados2 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 2
                                      select rowper;
                    var fila2 = resultados2.FirstOrDefault();
                    modulePermisson.IdEditar = 2;
                    modulePermisson.Editar = fila2?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados3 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 3
                                      select rowper;
                    var fila3 = resultados3.FirstOrDefault();
                    modulePermisson.idCrear = 3;
                    modulePermisson.Crear = fila3?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados4 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 4
                                      select rowper;
                    var fila4 = resultados4.FirstOrDefault();
                    modulePermisson.idSolicitar = 4;
                    modulePermisson.Solicitar = fila4?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados5 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 5
                                      select rowper;
                    var fila5 = resultados5.FirstOrDefault();
                    modulePermisson.idAutorizar = 5;
                    modulePermisson.Autorizar = fila5?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados6 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 6
                                      select rowper;
                    var fila6 = resultados6.FirstOrDefault();
                    modulePermisson.idBorrar = 6;
                    modulePermisson.Borrar = fila6?.Field<int?>("Permisson") == null ? 0 : 1;


                    var Hijos = from DataRow fila in tablaDatos.Rows
                                where (int)fila["IdModulo"] == int.Parse(row.Columna1.ToString())
                                select fila;

                    if (Hijos.Any())
                    {
                        List<ModulePermisson> modulePermissonsListHijos = new List<ModulePermisson>();
                        // Llamar al método recursivo para procesar los hijos
                        ProcesarHijosRecursivo2(Hijos.ToArray(), modulePermissonsListHijos, tablaDatos, tablaDatosPermisos);
                        modulePermisson.modulePermissons = modulePermissonsListHijos;
                    }


                    modulePermissonsList.Add(modulePermisson);

                }


                response.modules = modulePermissonsList;


                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }
        public ModulePermissonResponse GetModuleDetailPermissonSpecial(string userId, int View)
        {
            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlCommand commandPermisos = null;
            SqlDataReader reader = null;
            SqlDataReader readerPermisos = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Modules_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                // command.Parameters.Add(new SqlParameter("@IdParentModulo", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(idParentModulo) });
                reader = command.ExecuteReader();
                DataTable tablaDatos = new DataTable();
                tablaDatos.Load(reader);


                commandPermisos = new SqlCommand(DataObjects.Usp_Security_PermissonSpecial_Get)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                commandPermisos.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = View });
                commandPermisos.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = userId });
                readerPermisos = commandPermisos.ExecuteReader();
                DataTable tablaDatosPermisos = new DataTable();
                tablaDatosPermisos.Load(readerPermisos);



                var Master = (from DataRow fila in tablaDatos.Rows
                              where (int)fila["Nivel"] == 1
                              select new { Columna1 = fila["IdModulo"], Columna2 = fila["TitleModulo"] }).Distinct();

                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (var row in Master)
                {
                    ModulePermisson modulePermisson = new ModulePermisson();
                    modulePermisson.Idmodulo = int.Parse(row.Columna1.ToString());
                    modulePermisson.TitleModulo = row.Columna2.ToString();




                    //Modificar permisos desde base de datos
                    modulePermisson.idVer = 1;
                    var resultados1 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 1
                                      select rowper;
                    var fila1 = resultados1.FirstOrDefault();
                    modulePermisson.Ver = fila1?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados2 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 2
                                      select rowper;
                    var fila2 = resultados2.FirstOrDefault();
                    modulePermisson.IdEditar = 2;
                    modulePermisson.Editar = fila2?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados3 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 3
                                      select rowper;
                    var fila3 = resultados3.FirstOrDefault();
                    modulePermisson.idCrear = 3;
                    modulePermisson.Crear = fila3?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados4 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 4
                                      select rowper;
                    var fila4 = resultados4.FirstOrDefault();
                    modulePermisson.idSolicitar = 4;
                    modulePermisson.Solicitar = fila4?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados5 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 5
                                      select rowper;
                    var fila5 = resultados5.FirstOrDefault();
                    modulePermisson.idAutorizar = 5;
                    modulePermisson.Autorizar = fila5?.Field<int?>("Permisson") == null ? 0 : 1;

                    var resultados6 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      && rowper.Field<int>("IdPermisson") == 6
                                      select rowper;
                    var fila6 = resultados6.FirstOrDefault();
                    modulePermisson.idBorrar = 6;
                    modulePermisson.Borrar = fila6?.Field<int?>("Permisson") == null ? 0 : 1;


                    var Hijos = from DataRow fila in tablaDatos.Rows
                                where (int)fila["IdModulo"] == int.Parse(row.Columna1.ToString())
                                select fila;

                    if (Hijos.Any())
                    {
                        List<ModulePermisson> modulePermissonsListHijos = new List<ModulePermisson>();
                        // Llamar al método recursivo para procesar los hijos
                        ProcesarHijosRecursivo2(Hijos.ToArray(), modulePermissonsListHijos, tablaDatos, tablaDatosPermisos);
                        modulePermisson.modulePermissons = modulePermissonsListHijos;
                    }


                    modulePermissonsList.Add(modulePermisson);

                }


                response.modules = modulePermissonsList;


                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }


        //*************************************************************************************
        //*************************************************************************************
        //****Nuevo Medoto
        public ModulePermissonResponse2 GetModuleDetailPermissonSpecial2(string userId, int View)
        {
            ModulePermissonResponse2 response = new ModulePermissonResponse2();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlCommand commandPermisos = null;
            SqlDataReader reader = null;
            SqlDataReader readerPermisos = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Modules_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                reader = command.ExecuteReader();
                DataTable tablaDatos = new DataTable();
                tablaDatos.Load(reader);


                commandPermisos = new SqlCommand(DataObjects.Usp_Security_PermissonSpecial_Get)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                commandPermisos.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = View });
                commandPermisos.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = userId });
                readerPermisos = commandPermisos.ExecuteReader();
                DataTable tablaDatosPermisos = new DataTable();
                tablaDatosPermisos.Load(readerPermisos);



                var Master = (from DataRow fila in tablaDatos.Rows
                              where (int)fila["Nivel"] == 1
                              select new { Columna1 = fila["IdModulo"], Columna2 = fila["TitleModulo"] }).Distinct();

                List<ModulePermisson2> modulePermissonsList = new List<ModulePermisson2>();

                foreach (var row in Master)
                {
                    ModulePermisson2 modulePermisson = new ModulePermisson2();
                    modulePermisson.Idmodulo = int.Parse(row.Columna1.ToString());
                    modulePermisson.TitleModulo = row.Columna2.ToString();


                    //   modulePermisson.idVer = 1;
                    var resultados1 = from rowper in tablaDatosPermisos.AsEnumerable()
                                      where rowper.Field<int>("IdModule") == int.Parse(row.Columna1.ToString())
                                      //&& rowper.Field<int>("IdPermisson") == 1
                                      select rowper;

                    var fila1 = resultados1.FirstOrDefault();



                    var Hijos = from DataRow fila in tablaDatos.Rows
                                where (int)fila["IdModulo"] == int.Parse(row.Columna1.ToString())
                                select fila;

                    if (Hijos.Any())
                    {
                        List<ModulePermisson2> modulePermissonsListHijos = new List<ModulePermisson2>();
                        // Llamar al método recursivo para procesar los hijos
                        ProcesarHijosRecursivo22(Hijos.ToArray(), modulePermissonsListHijos, tablaDatos, tablaDatosPermisos);
                        modulePermisson.modulePermissons = modulePermissonsListHijos;
                    }


                    modulePermissonsList.Add(modulePermisson);

                }


                response.modules = modulePermissonsList;


                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }


        //*************************************************************************************
        //*************************************************************************************

        public ModulePermissonExtendedResponse GetSpecialPermissonExtendedByUser(string userId, int View)
        {
            ModulePermissonExtendedResponse response = new ModulePermissonExtendedResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_PermissonSpecial_Get)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(View) });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();
                DataTable tablaDatosPermisos = new DataTable();
                tablaDatosPermisos.Load(reader);


                List<PermissonSpecialExtended> modulePermissonsList = new List<PermissonSpecialExtended>();

                foreach (DataRow row in tablaDatosPermisos.Rows)
                {
                    PermissonSpecialExtended modulePermisson = new PermissonSpecialExtended();
                    modulePermisson.UserId = row["UserId"].ToString();
                    modulePermisson.IdModulo = int.Parse(row["IdModulo"].ToString());
                    modulePermisson.Modulo = row["Modulo"].ToString();
                    modulePermisson.SubModulo = row["SubModulo"].ToString();
                    modulePermisson.IdPermisson = int.Parse(row["IdPermisson"].ToString());
                    modulePermisson.PermissonName = row["PermissonName"].ToString();
                    modulePermisson.InitialDate = DateTime.Parse(row["InitialDate"].ToString());
                    modulePermisson.FinalDate = DateTime.Parse(row["FinalDate"].ToString());
                    modulePermisson.Status = bool.Parse(row["Status"].ToString());

                    modulePermissonsList.Add(modulePermisson);

                }


                response.permissonSpecialExtended = modulePermissonsList;


                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }


        public List<RolesAccionAlta> GetSpecialPermissonExtendedByUserNew(string userId, int View)
        {

            List<RolesAccionAlta> ListRolesAccionAlta = new List<RolesAccionAlta>();

            ModulePermissonResponse response = new ModulePermissonResponse();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_security_GetModuleNew)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(11) });
                command.Parameters.Add(new SqlParameter("@TypeModule", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(0) });
                command.Parameters.Add(new SqlParameter("@RolId", SqlDbType.NVarChar) { SqlValue = TypeHelper.ValidateString("") });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();

                DataTable tablaDatos = new DataTable();

                tablaDatos.Load(reader);

                reader.Close();
                command.Dispose();
                //RolesAccionAlta
                DataRow[] filteredRowsModulo = tablaDatos.Select("IdTypeModulo = 100");

                List<ModulePermisson> modulePermissonsList = new List<ModulePermisson>();

                foreach (DataRow rowModulo in filteredRowsModulo)
                {

                    RolesAccionAlta rolesAccionAlta = new RolesAccionAlta();
                    rolesAccionAlta.IdModule = rowModulo.Field<int>("IdModulo");
                    rolesAccionAlta.NameModulo = rowModulo.Field<string>("NombreModulo");



                    DataRow[] filteredRowsSubmdulos = tablaDatos.Select("IdTypeModulo = 200 AND IdParentModulo = " + rowModulo.Field<int>("IdModulo").ToString());
                    List<Submodule> submodulesList = new List<Submodule>();
                    foreach (DataRow rowSubmodulo in filteredRowsSubmdulos)
                    {
                        Submodule submodule = new Submodule();
                        submodule.NameSubmodulo = rowSubmodulo.Field<string>("NombreModulo");

                        DataRow[] filteredRowsView = tablaDatos.Select("IdTypeModulo = 300 AND IdParentModulo = " + rowSubmodulo.Field<int>("IdModulo").ToString());
                        List<Views> viewList = new List<Views>();
                        foreach (DataRow rowView in filteredRowsView)
                        {
                            Views views = new Views();
                            views.ViewName = rowView.Field<string>("NombreModulo");

                            DataRow[] filteredRowsAccions = tablaDatos.Select("IdTypeModulo = 400 AND IdParentModulo = " + rowView.Field<int>("IdModulo").ToString());
                            List<ActionItem> actionsList = new List<ActionItem>();
                            foreach (DataRow rowAction in filteredRowsAccions)
                            {
                                ActionItem actions = new ActionItem();
                                actions.Name = rowAction.Field<string>("NombreModulo");
                                actions.IdAction = rowAction.Field<int>("IdModulo");
                                if (actions.IdAction.Equals(28))
                                {
                                    var test = 1;
                                }
                                var dato = rowAction.Field<bool>("Status");
                                actions.StartDate = rowAction.Field<DateTime?>("InitialDate"); ;
                                actions.EndDate = rowAction.Field<DateTime?>("FinalDate"); ;
                                actions.Permisson = dato;
                                actionsList.Add(actions);
                            }
                            views.Acciones = actionsList;
                            viewList.Add(views);

                        }
                        submodule.Vistas = viewList;
                        submodulesList.Add(submodule);
                    }
                    rolesAccionAlta.Modules = submodulesList;
                    ListRolesAccionAlta.Add(rolesAccionAlta);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return ListRolesAccionAlta;
        }

        public ResponseBaseDto CreatePermissonSpecial(PermissonSpecialRequest request)
        {
            ResponseBaseDto response = new ResponseBaseDto
            {
                Success = true,
                ErrorList = new List<ErrorDto>()
            };
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                //*****************************************************************************************************************
                try
                {
                    var requestPermisson = (from n in request.PermissonsSpecial
                                            select new
                                            {
                                                IdPermissonSpecial = 0,
                                                n.UserId,
                                                n.IdModulo,
                                                n.IdPermisson,
                                                n.InitialDate,
                                                n.FinalDate,
                                                Status = 1,
                                            }).ToList();

                    DataTable PermissonSpecial = new DataTable();
                    PermissonSpecial.Columns.Add("IdPermissonSpecial", typeof(int));
                    PermissonSpecial.Columns.Add("UserId", typeof(string));
                    PermissonSpecial.Columns.Add("IdModulo", typeof(string));
                    PermissonSpecial.Columns.Add("IdPermisson", typeof(int));
                    PermissonSpecial.Columns.Add("InitialDate", typeof(DateTime));
                    PermissonSpecial.Columns.Add("FinalDate", typeof(int));
                    PermissonSpecial.Columns.Add("Status", typeof(bool));
                    PermissonSpecial = requestPermisson.ToList().AsDataTable();

                    using (SqlBulkCopy sbCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        sbCopy.ColumnMappings.Add("IdPermissonSpecial", "IdPermissonSpecial");
                        sbCopy.ColumnMappings.Add("UserId", "UserId");
                        sbCopy.ColumnMappings.Add("IdModulo", "IdModulo");
                        sbCopy.ColumnMappings.Add("IdPermisson", "IdPermisson");
                        sbCopy.ColumnMappings.Add("InitialDate", "InitialDate");
                        sbCopy.ColumnMappings.Add("FinalDate", "FinalDate");
                        sbCopy.ColumnMappings.Add("Status", "Status");
                        sbCopy.BulkCopyTimeout = 0;
                        sbCopy.BatchSize = 10000;
                        sbCopy.DestinationTableName = TableObjects.PermissonSpecial;
                        sbCopy.WriteToServer(PermissonSpecial);
                        sbCopy.Close();
                    }
                }
                catch (Exception exception)
                {
                    response.Code = ErrorFlag.ERROR.ToString();
                    response.Success = false;
                    response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.ModuloAccesos), TechnicalMessage = exception.Message });
                }


                //SqlCommand command2 = null;

                //command2 = new SqlCommand(DataObjects.Usp_Security_Rol_UDP)
                //{
                //    Connection = connection,
                //    CommandType = CommandType.StoredProcedure
                //};
                //command2.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar) { SqlValue = strRolId[0] });
                //command2.Parameters.Add(new SqlParameter("@RolName", SqlDbType.VarChar) { SqlValue = request.RolName });
                //command2.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { SqlValue = request.Description });
                //command2.Parameters.Add(new SqlParameter("@Type", SqlDbType.VarChar) { SqlValue = request.Type });
                //command2.Transaction = transaction;
                //command2.ExecuteScalar();


                response.Message = "Proceso completado";
                transaction.Commit();
            }
            catch (Exception exception)
            {
                response.Code = ErrorFlag.ERROR.ToString();
                response.Success = false;
                response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.PermissonRoles), TechnicalMessage = exception.Message });
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return response;
        }

        public ResponseBaseDto CreatePermissonSpecial2(PermissonSpecialRequest2 request)
        {
            ResponseBaseDto response = new ResponseBaseDto
            {
                Success = true,
                ErrorList = new List<ErrorDto>()
            };
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                //*****************************************************************************************************************
                try
                {
                    var requestPermisson = (from n in request.PermissonsSpecial
                                            select new
                                            {
                                                IdPermissonSpecial = 0,
                                                n.UserId,
                                                n.IdModulo,
                                                IdPermisson = 0,
                                                n.InitialDate,
                                                n.FinalDate,
                                                Status = 1,
                                            }).ToList();

                    DataTable PermissonSpecial = new DataTable();
                    PermissonSpecial.Columns.Add("IdPermissonSpecial", typeof(int));
                    PermissonSpecial.Columns.Add("UserId", typeof(string));
                    PermissonSpecial.Columns.Add("IdModulo", typeof(string));
                    PermissonSpecial.Columns.Add("IdPermisson", typeof(int));
                    PermissonSpecial.Columns.Add("InitialDate", typeof(DateTime));
                    PermissonSpecial.Columns.Add("FinalDate", typeof(int));
                    PermissonSpecial.Columns.Add("Status", typeof(bool));
                    PermissonSpecial = requestPermisson.ToList().AsDataTable();

                    using (SqlBulkCopy sbCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        sbCopy.ColumnMappings.Add("IdPermissonSpecial", "IdPermissonSpecial");
                        sbCopy.ColumnMappings.Add("UserId", "UserId");
                        sbCopy.ColumnMappings.Add("IdModulo", "IdModulo");
                        sbCopy.ColumnMappings.Add("IdPermisson", "IdPermisson");
                        sbCopy.ColumnMappings.Add("InitialDate", "InitialDate");
                        sbCopy.ColumnMappings.Add("FinalDate", "FinalDate");
                        sbCopy.ColumnMappings.Add("Status", "Status");
                        sbCopy.BulkCopyTimeout = 0;
                        sbCopy.BatchSize = 10000;
                        sbCopy.DestinationTableName = TableObjects.PermissonSpecial;
                        sbCopy.WriteToServer(PermissonSpecial);
                        sbCopy.Close();
                    }
                }
                catch (Exception exception)
                {
                    response.Code = ErrorFlag.ERROR.ToString();
                    response.Success = false;
                    response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.ModuloAccesos), TechnicalMessage = exception.Message });
                }


                response.Message = "Proceso completado";
                transaction.Commit();
            }
            catch (Exception exception)
            {
                response.Code = ErrorFlag.ERROR.ToString();
                response.Success = false;
                response.ErrorList.Add(new ErrorDto() { Code = ErrorMessage.DATA_ERRORCODE, Message = string.Format(ErrorMessage.BULK_UPDATE_TABLE, TableObjects.PermissonRoles), TechnicalMessage = exception.Message });
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return response;
        }

        public ResponseBaseDto DelSpecialPermissonExtendedByUser(string userId, int View)
        {
            ResponseBaseDto response = new ResponseBaseDto();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_PermissonSpecial_Del)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(View) });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                var rows = command.ExecuteNonQuery();

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }

        public ResponseBaseDto DelSpecialPermissonExtendedByUserAccion(string userId, int View, int moduloId)
        {
            ResponseBaseDto response = new ResponseBaseDto();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_PermissonSpecial_Del)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(View) });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                command.Parameters.Add(new SqlParameter("@IdModulo", SqlDbType.VarChar) { SqlValue = TypeHelper.ValidateInt(moduloId) });
                var rows = command.ExecuteNonQuery();

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return response;
        }



        public PermissonAccess GetUserPerAccesss(string userId)
        {
            PermissonAccess response = new PermissonAccess();
            //SqlConnection connection = new SqlConnection(DataHelper.GetConnectionString());
            //SqlCommand command = null;
            //SqlDataReader reader = null;

            //try
            //{
            //    connection.Open();

            //    command = new SqlCommand(DataObjects.Usp_Security_Modulo_GET)
            //    {
            //        Connection = connection,
            //        CommandType = CommandType.StoredProcedure
            //    };
            //    command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(2) });
            //    command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar) { SqlValue = TypeHelper.ValidateString(userId) });
            //    reader = command.ExecuteReader();

            //    DataTable tablaDatos = new DataTable();

            //    tablaDatos.Load(reader);

            //    IEnumerable<DataRow> enumerableRowsTodos = tablaDatos.AsEnumerable();

            //    var modulosPadre = from row in enumerableRowsTodos
            //                       where row.Field<int?>("IdParentModulo") == null
            //                       select row;
            //    List<PerAccess> listModulo = new List<PerAccess>();

            //    var Acciones = from rowSub in enumerableRowsTodos
            //                     where rowSub.Field<int?>("IdTypeModulo") == 400 && rowSub.Field<int?>("IdParentModulo") == row.Field<int>("IdModulo")
            //                     select rowSub;


            //    foreach (DataRow row in modulosPadre)
            //    {

            //        var submodulos = from rowSub in enumerableRowsTodos
            //                         where rowSub.Field<int?>("IdTypeModulo") == 200 && rowSub.Field<int?>("IdParentModulo") == row.Field<int>("IdModulo")
            //                         select rowSub;

            //        PerAccess item = new PerAccess();
            //        item.idModulo = row.Field<int>("IdModulo");
            //        item.idModuloParent = row.Field<int?>("IdParentModulo") == null ? 0 : row.Field<int>("IdParentModulo");
            //        item.moduloName = row.Field<string>("TitleModulo");
            //        listModulo.Add(item);
            //    }

            ////    response.perAccess = listModulo;
            //    reader.Close();
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
            //finally
            //{
            //    if (command != null)
            //    {
            //        command.Dispose();
            //    }

            //    if (connection.State != ConnectionState.Closed)
            //    {
            //        connection.Close();
            //        connection.Dispose();
            //    }
            //    if (reader != null)
            //    {
            //        if (!reader.IsClosed)
            //        {
            //            reader.Close();
            //        }
            //    }
            //}

            return response;
        }

        public List<Modulo> ConstruirEstructuraJerarquica(string userId)
        {

            PermissonAccess response = new PermissonAccess();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = null;
            SqlDataReader reader = null;

            var modulos = new List<Modulo>();

            try
            {
                connection.Open();

                command = new SqlCommand(DataObjects.Usp_Security_Modulo_GET)
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@View", SqlDbType.Int) { SqlValue = TypeHelper.ValidateInt(3) });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar) { SqlValue = TypeHelper.ValidateString(userId) });
                reader = command.ExecuteReader();

                DataTable tablaDatos = new DataTable();



                tablaDatos.Load(reader);
                // Mapear filas a objetos planos para facilitar búsquedas
                var listaPlanos = tablaDatos.AsEnumerable().Select(row => new
                {
                    IdModulo = row.Field<int>("IdModulo"),
                    IdParentModulo = row.Field<int?>("IdParentModulo"),
                    TitleModulo = row.Field<string>("TitleModulo"),
                    DescriptionModulo = row.Field<string>("DescriptionModulo"),
                    IdTypeModulo = row.Field<int>("IdTypeModulo")
                }).ToList();






                foreach (var item in listaPlanos)
                {
                    switch (item.IdTypeModulo)
                    {
                        case 100: // Módulo
                            modulos.Add(new Modulo
                            {
                                IdModulo = item.IdModulo,
                                Nombre = item.TitleModulo
                            });
                            break;

                        case 200: // Submodulo
                            var moduloPadre = modulos.FirstOrDefault(m => m.IdModulo == item.IdParentModulo);
                            if (moduloPadre != null)
                            {
                                moduloPadre.Submodulos.Add(new Submodulo
                                {
                                    IdModulo = item.IdModulo,
                                    Nombre = item.TitleModulo
                                });
                            }
                            break;

                        case 300: // Pantalla
                            foreach (var modulo in modulos)
                            {
                                var submoduloPadre = modulo.Submodulos.FirstOrDefault(s => s.IdModulo == item.IdParentModulo);
                                if (submoduloPadre != null)
                                {
                                    submoduloPadre.Pantallas.Add(new Pantalla
                                    {
                                        IdModulo = item.IdModulo,
                                        Nombre = item.TitleModulo
                                    });
                                }
                            }
                            break;

                        case 400: // Permisos
                            foreach (var modulo in modulos)
                            {
                                foreach (var submodulo in modulo.Submodulos)
                                {
                                    var pantallaPadre = submodulo.Pantallas.FirstOrDefault(p => p.IdModulo == item.IdParentModulo);
                                    if (pantallaPadre != null)
                                    {
                                        pantallaPadre.Permisos.Add(new Permiso
                                        {
                                            IdModulo = item.IdModulo,
                                            Titulo = item.TitleModulo,
                                            IdAccion = int.TryParse(item.DescriptionModulo, out int idAccion) ? idAccion : 0
                                        });
                                    }
                                }
                            }
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return modulos;



        }

    }

}



