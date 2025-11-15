using System;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Drawing.Printing;

namespace LLMTestApp
{
    #region Protocol structs
    public struct PageTranslationRequest
    {
        public string source_lang { get; set; }
        public string target_lang { get; set; }
        public string page_category { get; set; }
    }

    public struct NewPageResponse
    {
        public string link_to_page { get; set; }
        public string text { get; set; }
    }

    public struct ValidateTranslationRequest
    {
        public string source_lang { get; set; }
        public string target_lang { get; set; }
        public string original_text { get; set; }
        public string text { get; set; }
    }

    public struct TranslationCorrection
    {
        public string original { get; set; }
        public string corrected { get; set; }
    }

    public struct ValidateTranslationResponse
    {
        public List<TranslationCorrection> corrections { get; set; }
        public string GetCorrectionsAsString()
        {
            string all_corrections = new string("");
            foreach (var corr in corrections)
            {
                var current_line = string.Format("{0}{1}{2}", corr.original, Environment.NewLine, corr.corrected);
                all_corrections += string.Format("{0}{1}", current_line, Environment.NewLine);
            }
            return all_corrections;
        }
    }

    #endregion
    
    public partial class Form1 : Form
    {
        private string Endpoint = "http://localhost:5000";
        private string source_lang = "English";
        private string target_lang = "Japanese";
        private string text_to_be_translated = "";
        public Form1()
        {
            InitializeComponent();
        }

        private async Task<string> RequestNewTextToTranslate(string source_lang, string target_lang, string category)
        {
            PageTranslationRequest R = new PageTranslationRequest { source_lang = source_lang, target_lang = target_lang, page_category = category };
            string jsonString = JsonSerializer.Serialize(R);

            // Create an instance of HttpClient
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = String.Format("{0}/api/talk_to_llm?json={1}", Endpoint, jsonString);
                    using HttpResponseMessage response = await client.GetAsync(url);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        NewPageResponse Page = JsonSerializer.Deserialize<NewPageResponse>(jsonResponse);
                        text_to_be_translated = Page.text;
                        return Page.text;
                    }
                    
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error during GET request: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
            }

            return "";
        }

        private async Task<string> RequestTranslationValidation(string source_lang, string target_lang, string my_text)
        {
            ValidateTranslationRequest R = new ValidateTranslationRequest { source_lang = source_lang, target_lang = target_lang, original_text=text_to_be_translated, text = my_text };
            string jsonString = JsonSerializer.Serialize(R);

            // Create an instance of HttpClient
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = String.Format("{0}/api/talk_to_llm?json={1}", Endpoint, jsonString);
                    using HttpResponseMessage response = await client.GetAsync(url);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        ValidateTranslationResponse translationResponse = JsonSerializer.Deserialize<ValidateTranslationResponse>(jsonResponse);
                        return String.Format("{0}{1}", translationResponse.GetCorrectionsAsString(), Environment.NewLine);                        
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error during GET request: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
            }

            return "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    string textToValidate = "";
                    Invoke((MethodInvoker)delegate
                    {
                        textToValidate = richTextBox2.Text;
                    });

                    string ? Result = await RequestTranslationValidation(source_lang, target_lang, textToValidate);
                    if (Result != null)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            richTextBox3.Text = Result;
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(async() =>
            {
                try
                {
                    string? Result = await RequestNewTextToTranslate(source_lang, target_lang, "Gaming");
                    if (Result != null)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            richTextBox1.Text = Result;
                        });
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });         
        }
    }
}