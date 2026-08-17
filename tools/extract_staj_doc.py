import json
from pathlib import Path
from docx import Document

path = Path(r"C:\Users\sümeyye\Downloads\klu_staj_defteri_şablon_-__temmuz_2026__ (1).docx")
doc = Document(path)

days = []
for table_index, table in enumerate(doc.tables):
    if table_index < 15:
        continue
    rows = table.rows
    if len(rows) < 4:
        continue
    topic = rows[0].cells[1].text.strip()
    date = rows[1].cells[4].text.strip() if len(rows[1].cells) > 4 else ""
    body = rows[3].cells[0].text.strip()
    next_day = rows[4].cells[0].text.strip() if len(rows) > 4 else ""
    days.append({
        "table_index": table_index,
        "day": len(days) + 1,
        "topic": topic,
        "date": date,
        "body": body,
        "next_day_marker": next_day,
        "body_chars": len(body),
    })

out = Path("tools/staj_doc_extract.json")
out.write_text(json.dumps(days, ensure_ascii=False, indent=2), encoding="utf-8")
print(out)
