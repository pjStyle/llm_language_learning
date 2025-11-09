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
    public struct PageTranslation
    {
        public string source_lang { get; set; }
        public string target_lang { get; set; }
        public string page_category { get; set; }
    }

    public struct PageTranslationRequest
    {
        public PageTranslation new_page_request { get { return my_request; } }

        private PageTranslation my_request = new PageTranslation();
        public PageTranslationRequest(string source_lang, string target_lang, string page_category) 
        {
            my_request = new PageTranslation();
            my_request.source_lang = source_lang;
            my_request.target_lang = target_lang;
            my_request.page_category = page_category;
        }
    }

    public struct NewPage
    {
        public string link_to_page { get; set; }
        public string text { get; set; }
    }
    public struct NewPageResponse
    {       
        public NewPage new_page_response { get { return new_page; } set { new_page = value; } }
        private NewPage new_page = new NewPage();

        public NewPageResponse()
        {}
    }

    public struct TranslationRequest
    {
        public string text { get; set; }
    }
    public struct ValidateTranslationRequest
    {
        public TranslationRequest validate_translation_request { get { return request; } set { request = value; } }
        private TranslationRequest request = new TranslationRequest();

        public ValidateTranslationRequest(string my_translation)
        {
            request.text = my_translation;
        }
    }
    public struct TranslationReponse
    {
        public int grade{ get; set; }
        public string correction{ get; set; }
    }
    public struct ValidateTranslationResponse
    {
        public TranslationReponse validate_translation_response { get { return response; } set { response = value; } }
        private TranslationReponse response = new TranslationReponse();

        public ValidateTranslationResponse()
        { }
    }
    #endregion

    public partial class Form1 : Form
    {
        private string Endpoint = "http://localhost:5000";
        public Form1()
        {
            InitializeComponent();
        }

        private async Task<string> RequestNewTextToTranslate(string source_lang, string target_lang, string category)
        {
            PageTranslationRequest R = new PageTranslationRequest(source_lang, target_lang, category);          
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
                        return Page.new_page_response.text;
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

        private async Task<string> RequestTranslationValidation(string my_text)
        {
            ValidateTranslationRequest R = new ValidateTranslationRequest(my_text);
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
                        return String.Format("{0}/5 {1}{2}", translationResponse.validate_translation_response.grade, Environment.NewLine, translationResponse.validate_translation_response.correction);                        
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

                    string ? Result = await RequestTranslationValidation(textToValidate);
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
                    string? Result = await RequestNewTextToTranslate("English", "Dutch", "Gaming");
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