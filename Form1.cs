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
    public partial class Form1 : Form
    {
        conexionbd conexion = new conexionbd();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargarCategoria();                        
        }

        public void cargarCategoria()
        {
            try
            {
                List<clReporte> lista = new List<clReporte>();
                string cadenaCone = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                using (var con = new SqlConnection(cadenaCone))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string consulta = "select cp_id_catprod,cp_desc from catg_prod ";
                    var query = new SqlCommand(consulta, con);                                     
                    using (var dr = query.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            clReporte Entity = new clReporte();                           
                            Entity.categoria = dr.GetValue(dr.GetOrdinal("cp_desc")).ToString();
                            Entity.codigoCat = Convert.ToInt32(dr.GetValue(dr.GetOrdinal("cp_id_catprod")));
                            lista.Add(Entity);
                        }
                    }                    
                }
                //
                cboCategoria.DataSource = lista;
                // Hace el enlace del campo au_id para el valor
                cboCategoria.ValueMember = "codigoCat";
                // Hace el enlace del campo au_fname para el texto
                cboCategoria.DisplayMember = "categoria";
                // Llena el DropDownList con los datos de la fuente de datos
               
            }
            catch (Exception ex)
            {

            }
        }

        public void cargarProducto()
        {
            try
            {
                List<clReporte> lista = new List<clReporte>();
                string cadenaCone = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                using (var con = new SqlConnection(cadenaCone))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string consulta = @"select pro_id_produc,pro_desc from producto
                                            where cp_id_catprod = @codcat ";
                    var query = new SqlCommand(consulta, con);
                    int codCat = Int32.Parse(cboCategoria.SelectedValue.ToString());
                    query.Parameters.AddWithValue("@codcat", codCat);
                    using (var dr = query.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            clReporte Entity = new clReporte();
                            Entity.producto = dr.GetValue(dr.GetOrdinal("pro_desc")).ToString();
                            Entity.codigoProd = Convert.ToInt32(dr.GetValue(dr.GetOrdinal("pro_id_produc")));
                            lista.Add(Entity);
                        }
                    }
                }
                //
                //cboProducto.Items.Clear();
                cboProducto.DataSource = lista;
                // Hace el enlace del campo au_id para el valor
                cboProducto.ValueMember = "codigoProd";
                // Hace el enlace del campo au_fname para el texto
                cboProducto.DisplayMember = "producto";
                // Llena el DropDownList con los datos de la fuente de datos

            }
            catch (Exception ex)
            {

            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DateTime fechaDesde = dtpDesde.Value.Date;
            DateTime fechaHasta = dtpHasta.Value.Date;
            int codCate = Int32.Parse(cboCategoria.SelectedValue.ToString());
            List<clReporte> lista = new List<clReporte>();
                string consultaDiaria = @"
                       select ve_fecha,
                            ctp.cp_desc 'categoria',pr.pro_desc 'producto',isnull(sum(dv.dvp_cant),0) 'cantidad',isnull(sum(cpa.cp_montot),0) 'montototal'  
                              from venta v 
                            inner join cliente c on c.per_id_cli=v.per_id_cli
                            inner join [ det_vent_produ] dv on dv.ve_id_vent=v.ve_id_vent
                            inner join detproalma dp on dp.dpa_id_detproalm=dv.dpa_id_detproalm
                            inner join producto pr on pr.pro_id_produc=dp.pro_id_produc
                            inner join catg_prod ctp on ctp.cp_id_catprod=pr.cp_id_catprod
                            inner join comp_pago cpa on cpa.cp_id_cbp=v.cp_id_cbp
                            where v.ve_fecha between @fechadesde and @fechahasta
                             and ctp.cp_id_catprod =@codCate
                            group by ve_fecha,ctp.cp_desc ,pr.pro_desc
                            order by ve_fecha
 ";
            string consultaMensual = @"
                         select datename(month,@fechadesde)+'-'+convert(varchar(8),year(ve_Fecha)) 'nombreMes', year(ve_fecha) 'anio',
                            month(ve_Fecha) 'mes',
                            ctp.cp_desc 'categoria',pr.pro_desc 'producto',isnull(sum(dv.dvp_cant),0) 'cantidad',isnull(sum(cpa.cp_montot),0) 'montototal'  
                              from venta v 
                            inner join cliente c on c.per_id_cli=v.per_id_cli
                            inner join [ det_vent_produ] dv on dv.ve_id_vent=v.ve_id_vent
                            inner join detproalma dp on dp.dpa_id_detproalm=dv.dpa_id_detproalm
                            inner join producto pr on pr.pro_id_produc=dp.pro_id_produc
                            inner join catg_prod ctp on ctp.cp_id_catprod=pr.cp_id_catprod
                            inner join comp_pago cpa on cpa.cp_id_cbp=v.cp_id_cbp
                            where v.ve_fecha between @fechadesde and @fechahasta 
                             and ctp.cp_id_catprod =@codCate
                            group by ctp.cp_desc ,pr.pro_desc,month(ve_fecha),year(ve_Fecha)
                            order by 2,3
 ";
            string consultaAnual = @"
                         select   year(ve_fecha) 'anio' ,
                            ctp.cp_desc 'categoria',pr.pro_desc 'producto',isnull(sum(dv.dvp_cant),0) 'cantidad',isnull(sum(cpa.cp_montot),0) 'montototal' 
                              from venta v 
                            inner join cliente c on c.per_id_cli=v.per_id_cli
                            inner join [ det_vent_produ] dv on dv.ve_id_vent=v.ve_id_vent
                            inner join detproalma dp on dp.dpa_id_detproalm=dv.dpa_id_detproalm
                            inner join producto pr on pr.pro_id_produc=dp.pro_id_produc
                            inner join catg_prod ctp on ctp.cp_id_catprod=pr.cp_id_catprod
                            inner join comp_pago cpa on cpa.cp_id_cbp=v.cp_id_cbp
                            where v.ve_fecha between @fechadesde and @fechahasta
                              and ctp.cp_id_catprod =@codCate
                            group by ctp.cp_desc ,pr.pro_desc, year(ve_Fecha)
                            order by 1,3
 ";

            try
                {
              string cadenaCone=  ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                using (var con = new SqlConnection(cadenaCone))
                    {
                        if(con.State==ConnectionState.Closed)
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
                    query.Parameters.AddWithValue("@codCate", codCate);
                   
                        Chart1.Series.Clear();
                        
                        using (var dr = query.ExecuteReader())
                        {
                       
                            while (dr.Read())
                            {
                            clReporte Entity = new clReporte();
                          
                            Entity.producto = dr.GetValue(dr.GetOrdinal("producto")).ToString();
                            Entity.categoria = dr.GetValue(dr.GetOrdinal("categoria")).ToString();                                             
                            Entity.cantidad = Convert.ToInt32( dr.GetValue(dr.GetOrdinal("cantidad")));                          
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
                        if(lista.Count>0)
                        {
                            List<string> lProductos = lista.ToList().Select(x => x.producto).Distinct().ToList();
                            foreach (string sprod in lProductos)
                            {
                                if (rbDiario.Checked == true)
                                {

                                    Chart1.Series.Add(sprod);
                                List<String> x = (from l in lista
                                                          where l.producto == sprod                                                          
                                                          select l.fecha).ToList();
                                List<double> y = (from l in lista
                                                        where l.producto == sprod
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
                                                      where l.producto == sprod
                                                      select l.nombreMes).ToList();
                                    List<double> y = (from l in lista
                                                      where l.producto == sprod
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
                                                      where l.producto == sprod
                                                      select l.anio).ToList();
                                    List<double> y = (from l in lista
                                                      where l.producto == sprod
                                                      select l.monto).ToList();

                                    Chart1.Series[sprod].XValueType = ChartValueType.Int32;
                                    Chart1.Series[sprod].YValueType = ChartValueType.Double;

                                    Chart1.Series[sprod].IsValueShownAsLabel = true;
                                    Chart1.Series[sprod].ChartType = SeriesChartType.StackedColumn;
                                    Chart1.Series[sprod].Points.DataBindXY(x, y);
                                //Entity.anio = Convert.ToDateTime(dr["ve_fecha"].ToString()).Year;
                            }
                        }
                        

                          
                        }
                    }
                }
                catch (Exception ex)
                {
                     
                } 



        }

        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarProducto();
        }
    }
}
