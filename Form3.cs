using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SSDOTTONEPRF
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DateTime fechaDesde = dtpDesde.Value.Date;
            DateTime fechaHasta = dtpHasta.Value.Date;
            List<clReporte> lista = new List<clReporte>();
            string consultaDiaria = @"
                        select ve_fecha,
                        per_direc 'direccion',isnull(sum(dv.dvp_cant),0) 'cantidad',isnull(sum(cpa.cp_montot),0) 'montototal'  
                            from venta v 
                        inner join cliente c on c.per_id_cli=v.per_id_cli
                        inner join [ det_vent_produ] dv on dv.ve_id_vent=v.ve_id_vent
                        inner join comp_pago cpa on cpa.cp_id_cbp=v.cp_id_cbp
                        where v.ve_fecha between @fechadesde and @fechahasta
                        group by ve_fecha,per_direc
                        order by ve_fecha
 ";
            string consultaMensual = @"
                         select datename(month,@fechadesde)+'-'+convert(varchar(8),year(ve_Fecha)) 'nombreMes', year(ve_fecha) 'anio',
                        month(ve_Fecha) 'mes',
                         per_direc 'direccion',isnull(sum(dv.dvp_cant),0) 'cantidad',isnull(sum(cpa.cp_montot),0) 'montototal'  
                            from venta v 
                         inner join cliente c on c.per_id_cli=v.per_id_cli
                        inner join [ det_vent_produ] dv on dv.ve_id_vent=v.ve_id_vent
                        inner join comp_pago cpa on cpa.cp_id_cbp=v.cp_id_cbp
                        where v.ve_fecha between @fechadesde and @fechahasta
                         group by per_direc,month(ve_fecha),year(ve_Fecha)
                        order by 2,3
 ";
            string consultaAnual = @"
                         
                    select   year(ve_fecha) 'anio' ,
                     per_direc 'direccion',isnull(sum(dv.dvp_cant),0) 'cantidad',isnull(sum(cpa.cp_montot),0) 'montototal' 
                        from venta v 
                         inner join cliente c on c.per_id_cli=v.per_id_cli
                        inner join [ det_vent_produ] dv on dv.ve_id_vent=v.ve_id_vent
                        inner join comp_pago cpa on cpa.cp_id_cbp=v.cp_id_cbp
                        where v.ve_fecha between @fechadesde and @fechahasta
                    group by per_direc, year(ve_Fecha)
                    order by 1,2
 ";

            try
            {
                string cadenaCone = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                using (var con = new SqlConnection(cadenaCone))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string consulta = "";
                    if (rbDiario.Checked == true)
                    {
                        consulta = consultaDiaria;
                    }
                    if (rbMensual.Checked == true)
                    {
                        consulta = consultaMensual;
                    }
                    if (rbAnual.Checked == true)
                    {
                        consulta = consultaAnual;
                    }

                    var query = new SqlCommand(consulta, con);
                    query.Parameters.AddWithValue("@fechadesde", fechaDesde);
                    query.Parameters.AddWithValue("@fechahasta", fechaHasta);
                    Chart1.Series.Clear();

                    using (var dr = query.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            clReporte Entity = new clReporte();
                            Entity.direccion = dr.GetValue(dr.GetOrdinal("direccion")).ToString();                          
                            Entity.cantidad = Convert.ToInt32(dr.GetValue(dr.GetOrdinal("cantidad")));
                            Entity.monto = Convert.ToDouble(dr.GetValue(dr.GetOrdinal("montototal")));
                            if (rbDiario.Checked == true)
                            {
                                Entity.fecha = Convert.ToDateTime(dr["ve_fecha"].ToString()).ToShortDateString();
                            }
                            if (rbMensual.Checked == true)
                            {
                                Entity.anio = Int32.Parse(dr.GetValue(dr.GetOrdinal("anio")).ToString());
                                Entity.mes = Int32.Parse(dr.GetValue(dr.GetOrdinal("mes")).ToString());
                                Entity.nombreMes = dr.GetValue(dr.GetOrdinal("nombreMes")).ToString();

                            }
                            if (rbAnual.Checked == true)
                            {
                                Entity.anio = Int32.Parse(dr.GetValue(dr.GetOrdinal("anio")).ToString());
                            }
                            lista.Add(Entity);
                        }
                    }
                    if (lista.Count > 0)
                    {
                        List<string> lProductos = lista.ToList().Select(x => x.direccion).Distinct().ToList();
                        foreach (string sprod in lProductos)
                        {
                            if (rbDiario.Checked == true)
                            {

                                Chart1.Series.Add(sprod);
                                List<String> x = (from l in lista
                                                  where l.direccion == sprod
                                                  select l.fecha).ToList();
                                List<double> y = (from l in lista
                                                  where l.direccion == sprod
                                                  select l.monto).ToList();

                                Chart1.Series[sprod].XValueType = ChartValueType.String;
                                Chart1.Series[sprod].YValueType = ChartValueType.Double;

                                Chart1.Series[sprod].IsValueShownAsLabel = true;
                                Chart1.Series[sprod].ChartType = SeriesChartType.StackedColumn;
                                Chart1.Series[sprod].Points.DataBindXY(x, y);

                            }
                            if (rbMensual.Checked == true)
                            {
                                Chart1.Series.Add(sprod);
                                List<String> x = (from l in lista
                                                  where l.direccion == sprod
                                                  select l.nombreMes).ToList();
                                List<double> y = (from l in lista
                                                  where l.direccion == sprod
                                                  select l.monto).ToList();

                                Chart1.Series[sprod].XValueType = ChartValueType.String;
                                Chart1.Series[sprod].YValueType = ChartValueType.Double;

                                Chart1.Series[sprod].IsValueShownAsLabel = true;
                                Chart1.Series[sprod].ChartType = SeriesChartType.StackedColumn;
                                Chart1.Series[sprod].Points.DataBindXY(x, y);                               
                            }
                            if (rbAnual.Checked == true)
                            {
                                Chart1.Series.Add(sprod);
                                List<int> x = (from l in lista
                                               where l.direccion == sprod
                                               select l.anio).ToList();
                                List<double> y = (from l in lista
                                                  where l.direccion == sprod
                                                  select l.monto).ToList();

                                Chart1.Series[sprod].XValueType = ChartValueType.Int32;
                                Chart1.Series[sprod].YValueType = ChartValueType.Double;

                                Chart1.Series[sprod].IsValueShownAsLabel = true;
                                Chart1.Series[sprod].ChartType = SeriesChartType.StackedColumn;
                                Chart1.Series[sprod].Points.DataBindXY(x, y);                                
                            }
                        }



                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
