using System;
using Npgsql;
using System.Text;
//using System.Data;
//using System.Collections.Generic;

namespace pspgf{
internal static class Program{
    [STAThread]
    static void Main(){

        var conn_str = "Server=localhost;Port=5432;Database=postgres;User ID=postgres;";
        var querry = @"
SELECT
	a.mngcode,
	a.subnum,
	b.subj,
	c.C_amount,
	c.deliv,
	c.job_seq,
	d.cus_name
FROM (
	SELECT *
	FROM a
	WHERE orderid = 123417
	)a

INNER JOIN (
	SELECT *
	FROM b
	)b
	ON a.bid = b.bid

INNER JOIN (
	SELECT *
	FROM c
	)c
	ON a.bid = c.bid

INNER JOIN (
	SELECT *
	FROM d
	)d
	ON b.cusid = d.cusid
";

        using(NpgsqlConnection conn = new NpgsqlConnection(conn_str)){
            conn.Open();
            using(var cmd = new NpgsqlCommand(querry, conn)){
                var res = cmd.ExecuteReader();
                while(res.Read()){
                    Console.Write(res["job_seq"]+", ");
                    System.Console.Write(res["deliv"]+", ");
                    System.Console.Write(res["mngcode"]+", ");
                    System.Console.WriteLine(res["subj"]);
                }
            }
            System.Console.WriteLine("hn");
        }
    }
}

}
