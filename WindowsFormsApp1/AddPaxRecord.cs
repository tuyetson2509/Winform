﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.API_Helper;
using WindowsFormsApp1.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp1
{
    public partial class AddPaxRecord : Form
    {
        private static string token;
        public AddPaxRecord(string Token)
        {
            InitializeComponent();
            token = Token;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string apiUrl = $"{Helper.BaseURL}/{"booking/add-pax-record"}";

            await AddPax(apiUrl, textBox1.Text, textBox2.Text);
        }
        private async Task<string> AddPax(string apiUrl, string trip_id, string voyage_date)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var a = new PaxDetailsAdd[] { };
                        var data = new ResponseArray[] { };

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            PaxDetailsAdd details = new PaxDetailsAdd();
                            details.passport_no = row.Cells["Column1"].Value?.ToString();
                            details.passport_type = row.Cells["Column2"].Value?.ToString();
                            details.last_name = row.Cells["Column3"].Value?.ToString();
                            details.first_name = row.Cells["Column4"].Value?.ToString();
                            details.gender = row.Cells["Column5"].Value?.ToString();
                            details.nationality = row.Cells["Column6"].Value?.ToString();
                            details.dob = row.Cells["Column7"].Value?.ToString();
                            details.country_of_birth = row.Cells["Column8"].Value?.ToString();
                            details.travel_doc_exp_date = row.Cells["Column9"].Value?.ToString();
                            details.country_of_issue = row.Cells["Column10"].Value?.ToString();
                            details.country_of_residence = row.Cells["Column11"].Value?.ToString();
                            details.destination = row.Cells["Column12"].Value?.ToString();
                            details.ticket_no = row.Cells["Column13"].Value?.ToString();
                            details.remarks = row.Cells["Column14"].Value?.ToString();
                            details.customer_book_code = row.Cells["Column15"].Value?.ToString();
                            details.cabin = row.Cells["Column16"].Value?.ToString();
                            details.pax_type = row.Cells["Column17"].Value?.ToString();
                            details.bag_allowance = Convert.ToInt32(row.Cells["Column18"].Value);
                            details.additional_bag_allowance = Convert.ToInt32(row.Cells["Column19"].Value);
                            details.security_program_flag = row.Cells["Column20"].Value?.ToString();
                            details.membership_no = row.Cells["Column21"].Value?.ToString();
                            details.check_in = Convert.ToInt32(row.Cells["Column22"].Value);
                            details.source_check_in = row.Cells["Column23"].Value?.ToString();
                            details.ssr = new List<string>();
                            if (row.Cells["Column24"].Value != null) {

                                string[] values = row.Cells["Column24"].Value?.ToString().Split(',').Select(value => value.Trim()).ToArray();
                                foreach (string value in values)
                                {
                                    details.ssr.Add(value);
                                }
                            }

                            a = a.Concat(new PaxDetailsAdd[] { details }).ToArray();
                        }

                        Array.Resize(ref a, a.Length - 1);
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var requestData = new 
                        {
                            trip_id = trip_id,
                            voyage_date = voyage_date,
                            pax_details = a
                        };

                        string jsonRequestData = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

                        var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            JObject json = JObject.Parse(jsonResponse);
                            txterr_msg.Text = json["err_msg"].ToString();
                            txterr_num.Text = json["err_num"].ToString();

                            ResponseArrayFull[] datares = JsonConvert.DeserializeObject<ResponseArrayFull[]>(json["data"].ToString());
                            foreach (var dt in datares)
                            {
                                dataGridView2.Rows.Add(dt.out_num, dt.out_str, dt.boarding_pass_number, dt.pax_id);
                            }
                            return await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            string error = await response.Content.ReadAsStringAsync();
                            MessageBox.Show(error);
                            return error;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            }
    }
}
