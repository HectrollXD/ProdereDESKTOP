using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using System.Data.OracleClient;
using System.Drawing.Drawing2D;
using Str;
using System.Drawing.Imaging;

namespace PRODERE{
    public partial class Home : Form {
        static string ConnectionString = "DATA SOURCE = localhost/orcl; USER = PRODERE; PASSWORD = Prodere!01001100";
        OracleConnection conexion = new OracleConnection(ConnectionString);
        bool Listo_Para_Registrar = false, click = false, firmo = false;
        int xposs = -1, yposs = -1;
        Pen pluma;
        Graphics firma;
        Bitmap bpm;
        DateTime fecha;

        public Home(){
            InitializeComponent();
            pluma = new Pen(Color.Black, 2);
            pluma.StartCap = pluma.EndCap = LineCap.Round;
            bpm = new Bitmap(477, 147);
            firma = Graphics.FromImage(bpm);
            firma.Clear(Color.White);
            firma.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private void Home_Load(object sender, EventArgs e){
            Verificar_botones_de_libros();
            CodLibro.Focus();
            Tabla_de_libros();
            Tabla_de_computadoras();
        }

        private void CodLibro_KeyPress(object sender, KeyPressEventArgs e){
            if( (int)e.KeyChar == (int)Keys.Enter ){
                if( int.Parse(Libros()[0]) == 0 ){
                    label9.Visible = true;  label10.Visible = true;  CodLibro.Text = "";
                    Libro_Manualmente.Enabled = true;  Verificar_botones_de_libros();
                    Listo_Para_Registrar = false; CodAlumno.Focus();
                }
                else{
                    CodAlumno.Focus();
                    Titulo.Text = Libros()[1];  Ejemplar.Text = Libros()[3]; Editorial.Text = Libros()[2];
                    Listo_Para_Registrar = true;
                }
            }
        }

        private void CodAlumno_KeyPress(object sender, KeyPressEventArgs e){
            if ((int)e.KeyChar == (int)Keys.Enter) {
                if (int.Parse(Alumnos()[0]) == 0) {
                    label11.Visible = true; label12.Visible = true; CodAlumno.Text = "";
                    Alumno_Manualmente_Libros.Enabled = true; Verificar_botones_de_libros();
                    Listo_Para_Registrar = false;
                }
                else{
                    Nombre_de_alumno_libros.Text = Alumnos()[1];
                    Codigo_de_alumno_libros.Text = Alumnos()[2];
                    Carrera_de_alumno_libros.Text = Alumnos()[3];
                    Registrar_prestamo_de_libro();
                }
            }
        }

        private void Libro_Manualmente_Click(object sender, EventArgs e){
            Titulo.Enabled = true; Ejemplar.Enabled = true; Editorial.Enabled = true;
            label3.Enabled = true; label4.Enabled = true; label5.Enabled = true;
        }

        private void Alumno_Manualmente_Libros_Click(object sender, EventArgs e){
            Nombre_de_alumno_libros.Enabled = true; Codigo_de_alumno_libros.Enabled = true; Carrera_de_alumno_libros.Enabled = true;
            label6.Enabled = true; label7.Enabled = true; label8.Enabled = true;
            MessageBox.Show("Asegurate de colocar tu nombre empezando por apellidos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void Cancelar_registro_de_libro_Click(object sender, EventArgs e){ Resetear_Todo_Libros(); }

        private void Registrar_libro_Click(object sender, EventArgs e){ Registrar_prestamo_de_libro(); Tabla_de_libros(); }

        private void Entregar_Libro_Click(object sender, EventArgs e){
            label16.Enabled = true;  Numero_de_registro_libros.Enabled = true;
            Caja_de_Firma_libros.Enabled = true; Entregar_Libro.Enabled = false;
            Cancelar_Entrega_Libros.Enabled = true;
            Verificar_botones_de_libros();
        }

        private void Titulo_KeyUp(object sender, KeyEventArgs e){ Verificar_campos_libros(); }
        private void Ejemplar_KeyUp(object sender, KeyEventArgs e){ Verificar_campos_libros(); }
        private void Editorial_KeyUp(object sender, KeyEventArgs e){ Verificar_campos_libros(); }
        private void Nombre_de_alumno_libros_KeyUp(object sender, KeyEventArgs e){ Verificar_campos_libros(); }
        private void Codigo_de_alumno_libros_KeyUp(object sender, KeyEventArgs e){ Verificar_campos_libros(); }
        private void Carrera_de_alumno_libros_TextChanged(object sender, EventArgs e){ Verificar_campos_libros(); }


        private void Caja_de_Firma_libros_MouseDown(object sender, MouseEventArgs e){ //Caja de firmas
            click = true;  xposs = e.X;  yposs = e.Y;
            Borrar_Firma_Libros.Enabled = true;
            firmo = true;
            if ( Numero_de_registro_libros.Text != "" ) { Guardar_Entrega_Libros.Enabled = true; }
            Verificar_botones_de_libros();
        }
        private void Caja_de_Firma_libros_MouseMove(object sender, MouseEventArgs e){
            if(click == true){
                firma.DrawLine(pluma, new Point(xposs, yposs), e.Location);
                xposs = e.X;
                yposs = e.Y;
                Caja_de_Firma_libros.Image = bpm;
            }
        }
        private void Caja_de_Firma_libros_MouseUp(object sender, MouseEventArgs e){
            click = false;
            xposs = -1;
            yposs = -1;
        }
        private void Numero_de_registro_libros_KeyUp(object sender, KeyEventArgs e){
            if(firmo == true){
                Guardar_Entrega_Libros.Enabled = true;
                Verificar_botones_de_libros();
            }
        }
        private void Borrar_Firma_Libros_Click(object sender, EventArgs e){
            firma.Clear(Color.White);  Caja_de_Firma_libros.Image = bpm;
            Borrar_Firma_Libros.Enabled = false; Guardar_Entrega_Libros.Enabled = false;
            Verificar_botones_de_libros(); firmo = false;
        }
        private void Cancelar_Entrega_Libros_Click(object sender, EventArgs e){
            firma.Clear(Color.White); Caja_de_Firma_libros.Image = null;
            label16.Enabled = false; Numero_de_registro_libros.Enabled = false; Numero_de_registro_libros.Text = "";
            Entregar_Libro.Enabled = true; Caja_de_Firma_libros.Enabled = false;
            Borrar_Firma_Libros.Enabled = false; Cancelar_Entrega_Libros.Enabled = false;
            CodLibro.Focus(); firmo = false; Guardar_Entrega_Libros.Enabled = false;
            Verificar_botones_de_libros();
        }

        private void Guardar_Entrega_Libros_Click(object sender, EventArgs e){
            entregar_libro();

            firma.Clear(Color.White); Caja_de_Firma_libros.Image = null;
            label16.Enabled = false; Numero_de_registro_libros.Enabled = false; Numero_de_registro_libros.Text = "";
            Entregar_Libro.Enabled = true; Caja_de_Firma_libros.Enabled = false;
            Borrar_Firma_Libros.Enabled = false; Cancelar_Entrega_Libros.Enabled = false;
            CodLibro.Focus(); firmo = false; Guardar_Entrega_Libros.Enabled = false;
            Verificar_botones_de_libros();
        }





        /****-------------------------------------------------------prestamos de computadoras*/
        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                textBox8.Focus();
            }

        }
        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                if (int.Parse(Alumnos2()[0]) == 0)
                {
                    label19.Visible = true; label20.Visible = true; textBox8.Text = "";
                    button3.Enabled = true; Verificar_botones_de_libros();
                    Listo_Para_Registrar = false;
                }
                else
                {
                    textBox1.Text = Alumnos2()[1];
                    textBox2.Text = Alumnos2()[2];
                    comboBox2.Text = Alumnos2()[3];
                    Registrar_prestamo_de_computadoras();
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = true; textBox2.Enabled = true; comboBox2.Enabled = true;
            label6.Enabled = true; label7.Enabled = true; label8.Enabled = true;
            MessageBox.Show("Asegurate de colocar tu nombre empezando por apellidos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public void Registrar_prestamo_de_computadoras()
        {
            fecha = DateTime.Now;
            conexion.Open();
            OracleCommand Query = new OracleCommand("insertar_prestamos_de_computadoras", conexion);
            Query.CommandType = CommandType.StoredProcedure;
            Query.Parameters.Add("N", OracleType.NVarChar).Value = textBox9.Text;
            Query.Parameters.Add("Cod", OracleType.NVarChar).Value = textBox2.Text;
            Query.Parameters.Add("Nom", OracleType.NVarChar).Value = textBox1.Text;
            Query.Parameters.Add("Fe", OracleType.NVarChar).Value = fecha.ToString("dd/MM/yyyy");
            Query.Parameters.Add("Ho", OracleType.NVarChar).Value = fecha.ToString("hh:mm tt", CultureInfo.InvariantCulture);
            Query.ExecuteNonQuery();
            conexion.Close();
            MessageBox.Show("Te haz registrado correctamente.");
            Resetear_Todo_Libros();
            Tabla_de_computadoras();
            textBox9.Focus();
        }

        public void Tabla_de_computadoras(){
            OracleCommand Query = new OracleCommand("SELECT numero_de_prestamo_de_compu, numero_de_computadora, CODIGO_DEL_ALUMNO, NOMBRE_DEL_ALUMNO FROM PRESTAMOS_DE_COMPUTADORAS WHERE STATUS = 0 ORDER BY NUMERO_DE_PRESTAMO_DE_COMPU", conexion);
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = Query;
            DataTable tabla_compus = new DataTable();
            adaptador.Fill(tabla_compus);
            dataGridView1.DataSource = tabla_compus;
        }





        /*__________________________________________Funciones Super Necesarias___________________________________________*/
        public string[] Alumnos(){
            string[] datos = new string[4];
            byte iterador = 0;
            string Seleccion = "SELECT NOMBRE, CODIGO_DE_ALUMNO, CARRERA FROM ALUMNOS WHERE CODIGO_DE_ALUMNO = '" + CodAlumno.Text + "' AND ELIMINADO = 0";
            OracleCommand Query = new OracleCommand(Seleccion, conexion);
            conexion.Open();
            OracleDataReader rows = Query.ExecuteReader();
            while(rows.Read()){
                datos[1] = rows.GetString(0);
                datos[2] = rows.GetString(1);
                datos[3] = rows.GetString(2);
                iterador++;
            }
            conexion.Close();
            if(iterador == 0) { datos[0] = "0"; }
            else{ datos[0] = "1"; }
            return datos;
        }

        public string[] Alumnos2(){
            string[] datos = new string[4];
            byte iterador = 0;
            string Seleccion = "SELECT NOMBRE, CODIGO_DE_ALUMNO, CARRERA FROM ALUMNOS WHERE CODIGO_DE_ALUMNO = '" + textBox8.Text + "' AND ELIMINADO = 0";
            OracleCommand Query = new OracleCommand(Seleccion, conexion);
            conexion.Open();
            OracleDataReader rows = Query.ExecuteReader();
            while(rows.Read()){
                datos[1] = rows.GetString(0);
                datos[2] = rows.GetString(1);
                datos[3] = rows.GetString(2);
                iterador++;
            }
            conexion.Close();
            if(iterador == 0) { datos[0] = "0"; }
            else{ datos[0] = "1"; }
            return datos;
        }

        public string[] Libros(){
            string[] datos = new string[4];
            byte iterador = 0;
            string Seleccion = "SELECT TITULO, EDITORIAL, EJEMPLAR FROM LIBROS WHERE CODIGO_DE_LIBRO = '" + CodLibro.Text + "' AND ELIMINADO = 0";
            OracleCommand Query = new OracleCommand(Seleccion, conexion);
            conexion.Open();
            OracleDataReader rows = Query.ExecuteReader();
            while (rows.Read()){
                datos[1] = rows.GetString(0);
                datos[2] = rows.GetString(1);
                datos[3] = rows.GetInt32(2).ToString();
                iterador++;
            }
            conexion.Close();
            if(iterador == 0){ datos[0] = "0"; }
            else{ datos[0] = "1"; }
            return datos;
        }

        public void Registrar_prestamo_de_libro(){
            fecha = DateTime.Now;
            if( Listo_Para_Registrar == true ){
                conexion.Open();
                OracleCommand Query = new OracleCommand("insertar_prestamos_de_libros", conexion);
                Query.CommandType = CommandType.StoredProcedure;
                Query.Parameters.Add("Titulo", OracleType.NVarChar).Value = Titulo.Text;
                Query.Parameters.Add("Ejemplar", OracleType.NVarChar).Value = Ejemplar.Text;
                Query.Parameters.Add("CodigoDeAlumno", OracleType.NVarChar).Value = Codigo_de_alumno_libros.Text;
                Query.Parameters.Add("Nombre", OracleType.NVarChar).Value = Nombre_de_alumno_libros.Text;
                Query.Parameters.Add("Fecha", OracleType.NVarChar).Value = fecha.ToString("dd/MM/yyyy");
                Query.Parameters.Add("HoraDeEntrada", OracleType.NVarChar).Value = fecha.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                Query.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("Te haz registrado correctamente.");
                Resetear_Todo_Libros();
                Tabla_de_libros();
            }
        }

        public void entregar_libro(){
            //Guardar firma
            Image imagencopia = (Image)Caja_de_Firma_libros.Image.Clone();
            imagencopia.Save("C:/Firmas/firma.png", ImageFormat.Png);
            fecha = DateTime.Now;
            int entregado = 0;

            string seleccion = "SELECT STATUS FROM PRESTAMOS_DE_LIBROS WHERE NUMERO_DE_PRESTAMO_DE_LIBRO = '" + Numero_de_registro_libros.Text +"'";
            OracleCommand Query = new OracleCommand(seleccion, conexion);
            conexion.Open();
            OracleDataReader rows = Query.ExecuteReader();
            while (rows.Read()){
                entregado = rows.GetInt32(0);
            }
            conexion.Close();

            if(entregado == 0){
                conexion.Open();
                OracleCommand Query2 = new OracleCommand("entregar_prestamos_de_libros", conexion);
                Query2.CommandType = CommandType.StoredProcedure;
                Query2.Parameters.Add("HoraDeSalida", OracleType.NVarChar).Value = fecha.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                Query2.Parameters.Add("NumeroDeRegistroLibro", OracleType.Int32).Value = Numero_de_registro_libros.Text;
                Query2.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("Se ha registrado la entrega del libro exitosamente.");
                Tabla_de_libros();
            }
            else{
                MessageBox.Show("Este número de registro ya a sido entregado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void entregar_compu()
        {
            //Guardar firma
            Image imagencopia = (Image)pictureBox7.Image.Clone();
            imagencopia.Save("C:/Firmas/firma.png", ImageFormat.Png);
            fecha = DateTime.Now;
            int entregado = 0;

            string seleccion = "SELECT STATUS FROM PRESTAMOS_DE_COMPUTADORAS WHERE NUMERO_DE_PRESTAMO_DE_COMPU = '" + textBox3.Text + "'";
            OracleCommand Query = new OracleCommand(seleccion, conexion);
            conexion.Open();
            OracleDataReader rows = Query.ExecuteReader();
            while (rows.Read())
            {
                entregado = rows.GetInt32(0);
            }
            conexion.Close();

            if (entregado == 0)
            {
                conexion.Open();
                OracleCommand Query2 = new OracleCommand("entregar_prestamos_de_compus", conexion);
                Query2.CommandType = CommandType.StoredProcedure;
                Query2.Parameters.Add("HoraDeSalida", OracleType.NVarChar).Value = fecha.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                Query2.Parameters.Add("NumeroDeRegistroCompu", OracleType.Int32).Value = textBox3.Text;
                Query2.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("Se ha registrado la entrega de la computadora exitosamente.");
                Tabla_de_computadoras();
            }
            else
            {
                MessageBox.Show("Este número de registro ya a sido entregado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Tabla_de_libros(){
            OracleCommand Query = new OracleCommand("SELECT NUMERO_DE_PRESTAMO_DE_LIBRO, TITULO_DEL_LIBRO, CODIGO_DEL_ALUMNO, NOMBRE_DEL_ALUMNO FROM PRESTAMOS_DE_LIBROS WHERE STATUS = 0 ORDER BY NUMERO_DE_PRESTAMO_DE_LIBRO", conexion);
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = Query;
            DataTable tabla_libros = new DataTable();
            adaptador.Fill(tabla_libros);
            Tabla_de_prestamos_de_libros.DataSource = tabla_libros;
        }

        private void button7_Click(object sender, EventArgs e){
            label17.Enabled = true; textBox3.Enabled = true;
            pictureBox7.Enabled = true; button7.Enabled = false;
            button5.Enabled = true;
            Verificar_botones_de_libros();
        }

        private void button2_Click(object sender, EventArgs e){
            textBox8.Text = ""; textBox9.Text = "";
            label19.Visible = false; label20.Visible= false;
            textBox1.Enabled = false; textBox1.Text = ""; textBox2.Enabled = false; textBox2.Text = "";
            comboBox2.Enabled = false; comboBox2.Text = ""; button3.Enabled = false; button1.Enabled = false;
            textBox9.Focus();  Verificar_botones_de_libros();
        }

        private void button1_Click(object sender, EventArgs e){ Registrar_prestamo_de_computadoras(); }

        private void comboBox2_TextChanged(object sender, EventArgs e) { button1.Enabled = true; Verificar_botones_de_libros(); }

        private void button5_Click(object sender, EventArgs e)
        {
            firma.Clear(Color.White); pictureBox7.Image = null;
            label17.Enabled = false; textBox3.Enabled = false; textBox3.Text = "";
            button7.Enabled = true; pictureBox7.Enabled = false;
            button6.Enabled = false; button5.Enabled = false;
            textBox9.Focus(); firmo = false; button4.Enabled = false;
            Verificar_botones_de_libros();
        }

        private void pictureBox7_MouseDown(object sender, MouseEventArgs e)
        {
            click = true; xposs = e.X; yposs = e.Y;
            button6.Enabled = true;
            firmo = true;
            if (textBox3.Text != "") { button4.Enabled = true; }
            Verificar_botones_de_libros();
        }

        private void pictureBox7_MouseMove(object sender, MouseEventArgs e)
        {
            if (click == true)
            {
                firma.DrawLine(pluma, new Point(xposs, yposs), e.Location);
                xposs = e.X;
                yposs = e.Y;
                pictureBox7.Image = bpm;
            }
        }

        private void pictureBox7_MouseUp(object sender, MouseEventArgs e)
        {
            click = false;
            xposs = -1;
            yposs = -1;
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (firmo == true)
            {
                button4.Enabled = true;
                Verificar_botones_de_libros();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            entregar_compu();

            firma.Clear(Color.White); pictureBox7.Image = null;
            label17.Enabled = false; textBox3.Enabled = false; textBox3.Text = "";
            button7.Enabled = true; pictureBox7.Enabled = false;
            button6.Enabled = false; button5.Enabled = false;
            textBox9.Focus(); firmo = false; button4.Enabled = false;
            Verificar_botones_de_libros();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            firma.Clear(Color.White); pictureBox7.Image = bpm;
            button6.Enabled = false; button4.Enabled = false;
            Verificar_botones_de_libros(); firmo = false;
        }

        public void Verificar_botones_de_libros(){
            if (Libro_Manualmente.Enabled == true) { Libro_Manualmente.BackColor = Color.FromArgb(0, 102, 255); }
            else { Libro_Manualmente.BackColor = Color.FromArgb(210, 210, 210); }
            if (Alumno_Manualmente_Libros.Enabled == true) { Alumno_Manualmente_Libros.BackColor = Color.FromArgb(0, 102, 255); }
            else { Alumno_Manualmente_Libros.BackColor = Color.FromArgb(210, 210, 210); }
            if (Registrar_libro.Enabled == true) { Registrar_libro.BackColor = Color.FromArgb(0, 102, 255); }
            else { Registrar_libro.BackColor = Color.FromArgb(210, 210, 210); }
            if (Borrar_Firma_Libros.Enabled == true) { Borrar_Firma_Libros.BackColor = Color.FromArgb(241, 73, 10); }
            else { Borrar_Firma_Libros.BackColor = Color.FromArgb(210, 210, 210); }
            if (Cancelar_Entrega_Libros.Enabled == true) { Cancelar_Entrega_Libros.BackColor = Color.Red; }
            else { Cancelar_Entrega_Libros.BackColor = Color.FromArgb(210,210, 210); }
            if (Guardar_Entrega_Libros.Enabled == true){ Guardar_Entrega_Libros.BackColor = Color.FromArgb(0, 102, 255); }
            else { Guardar_Entrega_Libros.BackColor = Color.FromArgb(210, 210, 210); }
            if (Entregar_Libro.Enabled == true) { Entregar_Libro.BackColor = Color.FromArgb(0, 102, 255); }
            else { Entregar_Libro.BackColor = Color.FromArgb(210, 210, 210); }
            if (Caja_de_Firma_libros.Enabled == true) { Caja_de_Firma_libros.BackColor = Color.White; }
            else { Caja_de_Firma_libros.BackColor = Color.FromArgb(210, 210, 210);  }

            if (button3.Enabled == true) { button3.BackColor = Color.FromArgb(0, 102, 255); }
            else { button3.BackColor = Color.FromArgb(210, 210, 210); }
            if (button1.Enabled == true) { button1.BackColor = Color.FromArgb(0, 102, 255); }
            else { button1.BackColor = Color.FromArgb(210, 210, 210); }
            if (button7.Enabled == true) { button7.BackColor = Color.FromArgb(0, 102, 255); }
            else { button7.BackColor = Color.FromArgb(210, 210, 210); }
            if (button4.Enabled == true) { button4.BackColor = Color.FromArgb(0, 102, 255); }
            else { button4.BackColor = Color.FromArgb(210, 210, 210); }
            if (button6.Enabled == true) { button6.BackColor = Color.FromArgb(241, 73, 10); }
            else { button6.BackColor = Color.FromArgb(210, 210, 210); }
            if (button5.Enabled == true) { button5.BackColor = Color.Red; }
            else { button5.BackColor = Color.FromArgb(210, 210, 210); }
        }

        

        public void Resetear_Todo_Libros(){
            Listo_Para_Registrar = false;
            Libro_Manualmente.Enabled = false;  Alumno_Manualmente_Libros.Enabled = false; Registrar_libro.Enabled = false;
            CodLibro.Text = "";  CodAlumno.Text = "";  CodLibro.Focus();
            Titulo.Text = "";  Ejemplar.Text = ""; Editorial.Text = "";
            Nombre_de_alumno_libros.Text = "";  Codigo_de_alumno_libros.Text = "";  Carrera_de_alumno_libros.Text = "";
            Titulo.Enabled = false; Ejemplar.Enabled = false; Editorial.Enabled = false;
            label3.Enabled = false; label4.Enabled = false; label5.Enabled = false;
            Nombre_de_alumno_libros.Enabled = false; Codigo_de_alumno_libros.Enabled = false; Carrera_de_alumno_libros.Enabled = false;
            label6.Enabled = false; label7.Enabled = false; label8.Enabled = false;
            label9.Visible = false; label10.Visible = false; label11.Visible = false; label12.Visible = false;
            textBox8.Text = ""; textBox9.Text = "";
            label19.Visible = false; label20.Visible = false;
            textBox1.Enabled = false; textBox1.Text = ""; textBox2.Enabled = false; textBox2.Text = "";
            comboBox2.Enabled = false; comboBox2.Text = ""; button3.Enabled = false; button1.Enabled = false;
            Verificar_botones_de_libros();
        }

        public void Verificar_campos_libros(){
            if (Titulo.Text != "" && Ejemplar.Text != "" && Editorial.Text != "" && Nombre_de_alumno_libros.Text != "" && Codigo_de_alumno_libros.Text != "" && Carrera_de_alumno_libros.Text != ""){
                Registrar_libro.Enabled = true;
                Listo_Para_Registrar = true;
                Verificar_botones_de_libros();
            }
        }
    }
}