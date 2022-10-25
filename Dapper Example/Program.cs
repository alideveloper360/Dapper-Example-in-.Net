using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dapper_Example.DataModel;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Dapper_Example
{
    class plo
    {
        public string FirstName { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {

            //DifferentTypesParameter().GetAwaiter().GetResult();
            //MultipleOutputParameter().GetAwaiter().GetResult();

            //TwoDataSet().GetAwaiter().GetResult();

            //ScalarValue().GetAwaiter().GetResult();
            //SingleOutputParameter().GetAwaiter().GetResult();
            okok<plo, int> ok = new okok<plo, int>();
            var result = ok.SingleRecord().GetAwaiter().GetResult();

            if (result.Any())
            {
                for(int i = 0; i < result.Count(); i++)
                {
                    var val = result[i].FirstName;
                }
            }

        }

        public static async Task<int> DifferentTypesParameter()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
            {
                await con.OpenAsync();

                var param = new DynamicParameters();
                param.Add("@pIntParam", 404, dbType: DbType.Int64, direction: ParameterDirection.Input);
                param.Add("@pNvarcharParam", "Test",dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add("@pDecimalParam", 1.1234, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add("@pBitParam", true, dbType: DbType.Boolean, direction: ParameterDirection.Input);

                param.Add("@pOutParam", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var result = await con.QueryAsync("uspMultipleInputParamAndOutPutParam", param, commandType: CommandType.StoredProcedure);

                int outputValue1 = param.Get<int>("@pOutParam");
                return outputValue1;
            }
        }

        public static async Task<int> ScalarValue()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
            {
                await con.OpenAsync();
                var result = await con.QueryAsync("uspScalarValue", commandType: CommandType.StoredProcedure);

                string currentDir = Environment.CurrentDirectory + @"\ScalarValue.txt";

                //if (!File.Exists(currentDir))
                //{
                //    File.Create(currentDir);
                //}
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentDir, true))
                //{
                //    file.WriteLine(result);
                //    file.Close();
                //}
            }
            return 0;
        }

        public static async Task<int> MultipleOutputParameter()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
            {
                await con.OpenAsync();
                var param = new DynamicParameters();
                param.Add("@pOutParam1", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@pOutParam2", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var result = await con.QueryAsync("uspMultipleOutPutParam", param, commandType: CommandType.StoredProcedure);

                int outputValue1 = param.Get<int>("@pOutParam1");

                int outputValue2 = param.Get<int>("@pOutParam2");


                //string currentDir = Environment.CurrentDirectory + @"\MultipleOutPutParam.txt";

                //if (!File.Exists(currentDir))
                //{
                //    File.Create(currentDir);
                //}
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentDir, true))
                //{
                //    file.WriteLine(outputValue1 + outputValue2);
                //    file.Close();
                //}
            }
            return 0;
        }

        public static async Task<int> SingleOutputParameter()
        {
            var param = new DynamicParameters();
            param.Add("@pOutParam", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
            {
                await con.OpenAsync();                

                var result = await con.QueryMultipleAsync("uspSingleOutPutParam", param, commandType: CommandType.StoredProcedure);

                int outputValue = param.Get<int>("@pOutParam");

            }



            //string currentDir = Environment.CurrentDirectory + @"\SingleOutPutParam.txt";

            //if (!File.Exists(currentDir))
            //{
            //    File.Create(currentDir);
            //}
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentDir, true))
            //{
            //    file.WriteLine(outputValue);
            //    file.Close();
            //}
            return 0;
        }
        
        public static async Task<int> TwoDataSet()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
            {
                await con.OpenAsync();
                var result = await con.QueryMultipleAsync("uspMultipleDataSet", commandType: CommandType.StoredProcedure);


                var ListOne = (await result.ReadAsync<Info>()).ToList();
                var ListTwo = result.Read<Info>().ToList();



                //string currentDir = Environment.CurrentDirectory + @"\MultipleRecord.txt";

                //if (!File.Exists(currentDir))
                //{
                //    File.Create(currentDir);
                //}
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentDir, true))
                //{
                //    file.WriteLine(ListOne.ToString() + ListTwo.ToString());
                //    file.Close();
                //}

            }
            return 0;
        }





    }
     
    public class okok<T, U>
        where T : class, new()
        where U : struct
    {
        public async Task<List<T>> SingleRecord()
        {
            return await test.ExecuteCommand<List<T>>(async con =>
            {
                //await con.OpenAsync();
                return (await con.QueryAsync<T>("uspSingleDataSet", commandType: CommandType.StoredProcedure)).ToList();


                //var okp = JsonConvert.SerializeObject(ListOne);

                //return result;
            });
            //string currentDir = Environment.CurrentDirectory + @"\SingleRecord.txt";

            //if (!File.Exists(currentDir))
            //{
            //    File.Create(currentDir);
            //}
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentDir, true))
            //{
            //    file.WriteLine(result.ToString());
            //    file.Close();
            //}
            //return 0;
        }
    }

    public static class test
    {
        public async static Task<T> ExecuteCommand<T>(Func<IDbConnection, Task<T>> command)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
                {
                    await con.OpenAsync();
                    return await command(con);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{nameof(command)} WithConnection() experienced a SQL timeout with exception: {ex.Message}");
            }
            catch (SqlException ex)
            {
                throw new Exception($"{nameof(command)} WithConnection() experienced a SQL exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(command)} WithConnection() experienced a SQL exception: {ex.Message}");
            }
        }
    }

}
