using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projetoforms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await this.getAllVinhos();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await this.getVinhoById(int.Parse (this.textCodigo.Text)); //chamando método que busca vinho por id
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await this.RemoveVinho();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private string URI;
        private void Form1_Load(object sender, EventArgs e)
        {
            this.URI = "https://localhost:44396/api/Vinhos";
        }

        //método que traz todos os vinhos
        private async Task getAllVinhos()
        {
            using(var client = new HttpClient())
            {

                using (var response = await client.GetAsync(this.URI))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var VinhosJsonString = await response.Content.ReadAsStringAsync();

                        this.dataGridView1.DataSource = JsonConvert.DeserializeObject<Vinhos[]>(VinhosJsonString).ToList();
                    }
                    else
                    {
                        MessageBox.Show("Falha na Comunicação" + response.StatusCode);
                    }
                }
            }

        }

        private async void button3_Click(object sender, EventArgs e) //método do botão que chama o método que adiciona o objeto 
        {
            await this.AddVinho();
        }

        private async Task getVinhoById(int cod_vinho)
        {
            using (var client = new HttpClient())
            {
                BindingSource bsDados = new BindingSource();
                String endereco = this.URI + "/" + cod_vinho.ToString(); //url + parâmetro
                HttpResponseMessage response = await client.GetAsync(endereco);

                if (response.IsSuccessStatusCode)
                {
                    var VinhoJson = await response.Content.ReadAsStringAsync(); //colocar conteúdo em uma variável
                    bsDados.DataSource = JsonConvert.DeserializeObject<Vinhos>(VinhoJson);
                    this.dataGridView1.DataSource = bsDados;

                }
                else
                {
                    MessageBox.Show("Falha na busca:" + response.StatusCode);
                }
            }
        }

        private async Task RemoveVinho() //método que remove vinho do banco
        {
            int cod_vinho = Convert.ToInt32(this.textCodigo.Text);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.URI); //coloca no objeto client o endereço da web api
                HttpResponseMessage response = await client.DeleteAsync(String.Format("{0}/{1}", URI, cod_vinho)); //url e parâmetro
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Vinho Removido!");
                    await getAllVinhos();
                }
                else
                {
                    MessageBox.Show("Falha na remoção:" + response.StatusCode);
                }
            }

        }
        private async Task UpdateVinho() //método que atualiza vinho no banco
        {
            Vinhos vinho = new Vinhos();
            vinho.cod_vinho = Convert.ToInt32(this.textCodigo.Text);
            vinho.nome_vinho = this.textNome.Text;
            vinho.idade_vinho = Convert.ToInt32(this.textIdade.Text);
            vinho.preco_vinho = Convert.ToDecimal(this.textPreco.Text);


            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.PutAsJsonAsync(this.URI+"/"+vinho.cod_vinho, vinho); //url e cabeçalho
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Vinho atualizado!");
                    await getAllVinhos();
                }
                else
                {
                    MessageBox.Show("Falha na atualização:"+response.StatusCode);
                }
            }
        }
        
        private async Task AddVinho() //método que adiciona o objeto
        {
            Vinhos vinho = new Vinhos();
            vinho.nome_vinho = this.textNome.Text;
            vinho.idade_vinho = Convert.ToInt32(this.textIdade.Text);
            vinho.preco_vinho = Convert.ToDecimal(this.textPreco.Text);

            using (var client = new HttpClient())
            {
                var vinhoSerialized = JsonConvert.SerializeObject(vinho); //transforma objeto em Json
                var content = new StringContent(vinhoSerialized, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(this.URI, content);
            }

            await this.getAllVinhos();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            textCodigo.Text = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textNome.Text = this.dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textIdade.Text = this.dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textPreco.Text = this.dataGridView1.CurrentRow.Cells[3].Value.ToString();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await this.UpdateVinho();
        }
    }
}
