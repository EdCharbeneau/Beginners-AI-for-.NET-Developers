### 🔧 Updated Prompt: Sentiment Analysis with Star Rating

**Task:**  
Analyze the provided user review and return a structured sentiment analysis in JSON format.

**Instructions:**  
- Classify the overall sentiment: `positive`, `neutral`, or `negative`.  
- Assign a sentiment score between `0.0` and `1.0`.  
- Map the sentiment score to a `star_rating` from `1` to `5`.  
- Summarize the reasoning behind the sentiment classification.

**Output Schema:**
```json
{
  "overall_sentiment": "positive | neutral | negative",
  "sentiment_score": <float between 0.0 and 1.0>,
  "star_rating": <integer between 1 and 5>,
  "reasoning_summary": "<Concise explanation of sentiment classification, citing relevant phrases and tone indicators.>"
}
```

**Star Rating Mapping Guidelines:**
| Sentiment Score Range | Star Rating |
|------------------------|-------------|
| 0.00–0.19              | 1           |
| 0.20–0.39              | 2           |
| 0.40–0.59              | 3           |
| 0.60–0.79              | 4           |
| 0.80–1.00              | 5           |

**Example Output:**
```json
{
  "overall_sentiment": "positive",
  "sentiment_score": 0.9,
  "star_rating": 5,
  "reasoning_summary": "The review expresses strong positive feelings about the beans, mentioning it as one of the user's favorites and the excitement of receiving them, indicated by the phrase 'little happy dance'. The description of the roasting process leading to a 'delightfully rich cup' further emphasizes satisfaction, along with the mention of 'subtle spicy undertones', which adds to the overall positive sentiment."
}
```