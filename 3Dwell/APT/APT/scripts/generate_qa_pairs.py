import json
import os

# Đường dẫn đến qlcc_data.json
data_path = "qlcc_data.json"

# Kiểm tra file tồn tại
if not os.path.exists(data_path):
    raise FileNotFoundError(f"File {data_path} không tồn tại")

# Đọc dữ liệu
try:
    with open(data_path, 'r', encoding='utf-8') as f:
        data = json.load(f)
except json.JSONDecodeError:
    raise ValueError("File JSON không hợp lệ")

qa_pairs = []

# Menu chính
qa_pairs.append({
    "question": "Chung cư",
    "answer": "Intent: Hỏi menu chung cư"
})
qa_pairs.append({
    "question": "Căn hộ",
    "answer": "Intent: Hỏi menu căn hộ"
})
qa_pairs.append({
    "question": "Dịch vụ",
    "answer": "Intent: Hỏi menu dịch vụ"
})
qa_pairs.append({
    "question": "Tin tức",
    "answer": "Intent: Hỏi menu tin tức"
})

# Chung cư
qa_pairs.append({
    "question": "Tổng số chung cư?",
    "answer": "Intent: Hỏi tổng số chung cư"
})
qa_pairs.append({
    "question": "Danh sách chung cư",
    "answer": "Intent: Hỏi danh sách chung cư"
})

# Lựa chọn chung cư
for chungcu in data.get("ChungCus", []):
    qa_pairs.append({
        "question": f"Xem chung cư {chungcu['Ten']}",
        "answer": f"Intent: Xem thông tin chung cư {chungcu['Ten']}"
    })

# Căn hộ
for chungcu in data.get("ChungCus", []):
    qa_pairs.append({
        "question": f"Tổng số căn hộ của chung cư {chungcu['Ten']}?",
        "answer": f"Intent: Hỏi tổng số căn hộ chung cư {chungcu['Ten']}"
    })
    qa_pairs.append({
        "question": f"Danh sách căn hộ của chung cư {chungcu['Ten']}?",
        "answer": f"Intent: Hỏi danh sách căn hộ chung cư {chungcu['Ten']}"
    })

# Lựa chọn căn hộ
for canho in data.get("CanHos", []):
    chungcu = next((cc for cc in data.get("ChungCus", []) if cc['ID'] == canho['ID_ChungCu']), None)
    if chungcu:
        qa_pairs.append({
            "question": f"Xem căn hộ {canho['MaCan']} thuộc chung cư {chungcu['Ten']}",
            "answer": f"Intent: Xem thông tin căn hộ {canho['MaCan']}"
        })

# Dịch vụ
qa_pairs.append({
    "question": "Tổng số dịch vụ hiện có?",
    "answer": "Intent: Hỏi tổng số dịch vụ"
})
qa_pairs.append({
    "question": "Danh sách dịch vụ",
    "answer": "Intent: Hỏi danh sách dịch vụ"
})

# Lựa chọn dịch vụ
for service in data.get("Services", []):
    qa_pairs.append({
        "question": f"Xem dịch vụ {service['TenDichVu']}",
        "answer": f"Intent: Xem thông tin dịch vụ {service['TenDichVu']}"
    })

# Tin tức
qa_pairs.append({
    "question": "Tin tức mới nhất?",
    "answer": "Intent: Hỏi tin tức mới nhất"
})

# Cư dân - Hóa đơn và phản ánh
for user in data.get("Users", []):
    if user["LoaiNguoiDung"] == "Cư dân":
        user_id = user['ID']
        user_email = user['Email']
        # Hóa đơn
        invoices = [
            i for i in data.get('Invoices', [])
            if any(r['ID_NguoiDung'] == user_id and r['ID_CanHo'] == i['ID_CanHo'] for r in data.get('Residents', []))
        ]
        if invoices:
            qa_pairs.append({
                "question": f"Hóa đơn của {user_email}?",
                "answer": "Intent: Hỏi hóa đơn"
            })
            qa_pairs.append({
                "question": f"Hóa đơn chưa thanh toán của {user_email}?",
                "answer": "Intent: Hỏi hóa đơn chưa thanh toán"
            })
        # Phản ánh
        complaints = [c for c in data.get('Complaints', []) if c['ID_NguoiDung'] == user_id]
        if complaints:
            qa_pairs.append({
                "question": f"Phản ánh của {user_email}?",
                "answer": "Intent: Hỏi phản ánh"
            })
            qa_pairs.append({
                "question": f"Phản ánh chưa xử lý của {user_email}?",
                "answer": "Intent: Hỏi phản ánh chưa xử lý"
            })

# Ban quản lý - Tổng hợp
qa_pairs.append({
    "question": "Tổng số cư dân?",
    "answer": "Intent: Hỏi tổng số cư dân"
})
qa_pairs.append({
    "question": "Tổng số hóa đơn chưa thanh toán?",
    "answer": "Intent: Hỏi tổng số hóa đơn chưa thanh toán"
})
qa_pairs.append({
    "question": "Tổng số phản ánh chưa xử lý?",
    "answer": "Intent: Hỏi tổng số phản ánh chưa xử lý"
})

# Save updated qa_pairs.json
with open("qa_pairs.json", 'w', encoding='utf-8') as f:
    json.dump(qa_pairs, f, ensure_ascii=False, indent=2)

print(f"Đã tạo {len(qa_pairs)} cặp câu hỏi-trả lời trong qa_pairs.json")