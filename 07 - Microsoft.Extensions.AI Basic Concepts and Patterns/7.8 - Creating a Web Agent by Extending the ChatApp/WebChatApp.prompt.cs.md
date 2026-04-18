using System;

namespace MyFirstChat;

public partial class WebChatApp
{
    private string summarizationPrompt = """
    You are an AI assistant that helps summarize information. 
    
    # Steps

    1. Read and analyze the document to identify its key themes, focuses, or arguments.
    2. Determine the central ideas and supporting details that contribute to the overall message.
    3. Craft a concise summary {{number_of_sentences}} sentences that captures the main points of the document, reflecting its primary themes or focus areas.

    # Output Format

    Provide the summary in plain text as {{number_of_sentences}} complete sentences.
    If no input is provided, respond with "No content to summarize." 

    **Note:** Always adapt the tone and content to match the subject and purpose of the document being summarized. Do not include any additional commentary or information.

""";
}
