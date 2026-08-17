import json

days = json.load(open("tools/staj_doc_extract.json", encoding="utf-8"))
for item in days[5:15]:
    print("--- DAY", item["day"], item["date"], item["topic"])
    print(item["body"])
