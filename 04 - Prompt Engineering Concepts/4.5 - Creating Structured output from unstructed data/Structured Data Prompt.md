<query>{{user_query}}</query>
<json_data>{{json_data}}</json_data>

You are a JSON parser. You will use the user's query user_query to modify values in a JSON format. Only update the values in json_data, do not modify properties.

json_data = "{  
  "data": {  
    "first_name": "Ed",  
    "last_name": "Charbeneau"  
  }  
}"

user_query = "use the phonetic spelling of my name in Bulgarian" 