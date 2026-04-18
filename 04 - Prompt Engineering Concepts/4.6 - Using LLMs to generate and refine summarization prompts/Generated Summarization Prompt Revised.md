Categorize and tag documents based on the provided categories: Roast Level, Decaf, Process, and Flavor Notes. Use specific predefined keywords or phrases from the content to assign accurate tags within these categories. Leave a category blank if there is no relevant information.

# Instructions

- **Roast Level**: Identify and tag the roast level from the document (e.g., Light, Medium, Dark). Leave blank if not mentioned. Multiple levels is acceptable.
- **Decaf**: Tag as "Decaf" if explicitly stated in the content. Leave untapped if not applicable.
- **Process**: Extract details on coffee processing methods (e.g., Washed, Natural, Honey, Anaerobic). Leave blank if not present.
- **Flavor Notes**: List key flavor notes (e.g., Chocolate, Citrus, Berry) mentioned in the document. Separate multiple notes by commas.

# Output Format

Provide tags in structured JSON format with the following fields:
```json
{
  "Roast Levels": [ ],
  "Decaf": "Decaf or leave blank",
  "Process": "Value or leave blank",
  "Flavor Notes":  [ ]
}
```

# Example

**Input Document:**
"This light roast coffee offers vibrant citrus and berry notes, processed using the washed method. A delightful choice for those seeking a bright and complex profile."

**Output JSON:**
```json
{
  "Roast Level": "Light",
  "Decaf": "",
  "Process": "Washed",
  "Flavor Notes": "Citrus, Berry"
}
```

# Notes

- Ensure that all extracted tags align with the categories and any specific phrasing in the input documents.
- Use the exact keywords or synonymous terms mentioned in the document for accurate tagging.