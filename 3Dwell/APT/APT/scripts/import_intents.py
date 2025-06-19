import json
import time
import os
from google.cloud import dialogflow_v2 as dialogflow

# Kiểm tra biến môi trường
if not os.getenv('GOOGLE_APPLICATION_CREDENTIALS'):
    raise EnvironmentError("GOOGLE_APPLICATION_CREDENTIALS not set.")

# Đọc file qa_pairs.json
try:
    with open('qa_pairs.json', 'r', encoding='utf-8') as file:
        qa_pairs = json.load(file)
except FileNotFoundError:
    raise FileNotFoundError("qa_pairs.json not found.")

# Khởi tạo client Dialogflow
try:
    client = dialogflow.IntentsClient()
    project_id = 'apartmentchatbot-458712'
    parent = f"projects/{project_id}/agent"
except Exception as e:
    raise Exception(f"Failed to initialize Dialogflow client: {e}")

# Xóa Intents cũ
try:
    existing_intents = client.list_intents(parent=parent)
    for intent in existing_intents:
        client.delete_intent(name=intent.name)
        print(f'Deleted intent: {intent.display_name}')
        time.sleep(2)
except Exception as e:
    print(f'Error deleting intents: {e}')

# Tạo Intent cho mỗi cặp câu hỏi-câu trả lời
for index, qa in enumerate(qa_pairs):
    intent_name = f"Intent_{index}_{qa['question'][:50].replace('?', '').replace(' ', '_')}"
    answer = qa['answer'].lower()
    training_phrases = [{'parts': [{'text': qa['question']}]}]

    # Thêm training phrases bổ sung
    if qa['question'] == "Chung cư":
        training_phrases.extend([
            {'parts': [{'text': "Chung cư là gì"}]},
            {'parts': [{'text': "Thông tin chung cư"}]},
            {'parts': [{'text': "Chung cư nào"}]},
            {'parts': [{'text': "Cho tôi xem chung cư"}]},
            {'parts': [{'text': "Chung cu"}]}
        ])
    elif qa['question'] == "Căn hộ":
        training_phrases.extend([
            {'parts': [{'text': "Căn hộ là gì"}]},
            {'parts': [{'text': "Thông tin căn hộ"}]},
            {'parts': [{'text': "Căn hộ nào"}]},
            {'parts': [{'text': "Cho tôi xem căn hộ"}]},
            {'parts': [{'text': "Can ho"}]}
        ])
    elif qa['question'] == "Dịch vụ":
        training_phrases.extend([
            {'parts': [{'text': "Dịch vụ là gì"}]},
            {'parts': [{'text': "Thông tin dịch vụ"}]},
            {'parts': [{'text': "Dịch vụ nào"}]},
            {'parts': [{'text': "Cho tôi xem dịch vụ"}]},
            {'parts': [{'text': "Dich vu"}]}
        ])
    elif qa['question'] == "Tin tức":
        training_phrases.extend([
            {'parts': [{'text': "Tin tức là gì"}]},
            {'parts': [{'text': "Tin tức mới"}]},
            {'parts': [{'text': "Tin tức nào"}]},
            {'parts': [{'text': "Cho tôi xem tin tức"}]},
            {'parts': [{'text': "Tin tuc"}]},
            {'parts': [{'text': "Tin tức mới nhất"}]},
            {'parts': [{'text': "Tin tức mới nhất?"}]}
        ])
    elif qa['question'] == "Tổng số chung cư?":
        training_phrases.extend([
            {'parts': [{'text': "Tổng số chung cư"}]},
            {'parts': [{'text': "Có bao nhiêu chung cư"}]},
            {'parts': [{'text': "Số lượng chung cư"}]},
            {'parts': [{'text': "Tổng chung cư"}]}
        ])
    elif qa['question'] == "Tổng số dịch vụ hiện có?":
        training_phrases.extend([
            {'parts': [{'text': "Tổng số dịch vụ"}]},
            {'parts': [{'text': "Có bao nhiêu dịch vụ"}]},
            {'parts': [{'text': "Số lượng dịch vụ"}]}
        ])
    elif qa['question'].startswith("Danh sách căn hộ của chung cư"):
        training_phrases.extend([
            {'parts': [{'text': "Danh sách căn hộ của chung cư này"}]},
            {'parts': [{'text': "Căn hộ của chung cư này"}]},
            {'parts': [{'text': "Cho tôi xem danh sách căn hộ của chung cư này"}]},
            {'parts': [{'text': "Danh sách căn hộ chung cư này"}]},
            {'parts': [{'text': qa['question'].replace("Danh sách căn hộ của chung cư", "Căn hộ chung cư")}]}
        ])
    elif qa['question'].startswith("Xem căn hộ"):
        training_phrases.extend([
            {'parts': [{'text': qa['question'].replace("Xem căn hộ", "Căn hộ")}]},
            {'parts': [{'text': qa['question'].replace("Xem căn hộ", "Thông tin căn hộ")}]},
            {'parts': [{'text': qa['question'].replace("Xem căn hộ", "Cho tôi xem căn hộ")}]},
            {'parts': [{'text': qa['question'].replace("Xem căn hộ", "Xem thông tin căn hộ")}]}
        ])
    elif qa['question'].startswith("Xem dịch vụ"):
        training_phrases.extend([
            {'parts': [{'text': qa['question'].replace("Xem dịch vụ", "Dịch vụ")}]},
            {'parts': [{'text': qa['question'].replace("Xem dịch vụ", "Thông tin dịch vụ")}]},
            {'parts': [{'text': qa['question'].replace("Xem dịch vụ", "Cho tôi xem dịch vụ")}]}
        ])
    elif qa['question'] == "Hóa đơn":
        training_phrases.extend([
            {'parts': [{'text': "Hóa đơn của tôi"}]},
            {'parts': [{'text': "Xem hóa đơn"}]},
            {'parts': [{'text': "Thông tin hóa đơn"}]},
            {'parts': [{'text': "Hóa đơn chưa thanh toán"}]},
            {'parts': [{'text': "Hóa đơn nào"}]},
            {'parts': [{'text': "Cho tôi xem hóa đơn"}]},
            {'parts': [{'text': "Hoa don"}]}
        ])
    elif qa['question'] == "Phản ánh":
        training_phrases.extend([
            {'parts': [{'text': "Phản ánh của tôi"}]},
            {'parts': [{'text': "Xem phản ánh"}]},
            {'parts': [{'text': "Thông tin phản ánh"}]},
            {'parts': [{'text': "Phản ánh chưa xử lý"}]},
            {'parts': [{'text': "Phản ánh nào"}]},
            {'parts': [{'text': "Cho tôi xem phản ánh"}]},
            {'parts': [{'text': "Phan anh"}]}
        ])
    elif qa['question'].startswith("Tổng số cư dân"):
        training_phrases.extend([
            {'parts': [{'text': "Số lượng cư dân"}]},
            {'parts': [{'text': "Có bao nhiêu cư dân"}]},
            {'parts': [{'text': "Tổng cư dân"}]}
        ])

    intent = {
        'display_name': intent_name,
        'training_phrases': training_phrases,
        'messages': [
            {'text': {'text': [answer]}}
        ],
        'input_context_names': [],
        'output_contexts': []
    }
    try:
        response = client.create_intent(parent=parent, intent=intent)
        print(f'Created intent: {response.display_name}')
        time.sleep(2)
    except Exception as e:
        print(f'Error creating intent for {qa["question"]}: {e}')
        time.sleep(10)

# Tạo Welcome Intent
welcome_intent = {
    'display_name': 'Welcome_Intent',
    'training_phrases': [
        {'parts': [{'text': 'Xin chào'}]},
        {'parts': [{'text': 'Hi'}]},
        {'parts': [{'text': 'Hello'}]},
        {'parts': [{'text': 'Chào bot'}]},
        {'parts': [{'text': 'Chào'}]},
        {'parts': [{'text': 'Bắt đầu'}]}
    ],
    'messages': [
        {'text': {'text': ['intent: welcome']}}
    ]
}
try:
    response = client.create_intent(parent=parent, intent=welcome_intent)
    print(f'Created welcome intent: {response.display_name}')
    time.sleep(2)
except Exception as e:
    print(f'Error creating welcome intent: {e}')
    time.sleep(10)

# Tạo Fallback Intent
fallback_intent = {
    'display_name': 'Fallback_Intent',
    'is_fallback': True,
    'training_phrases': [
        {'parts': [{'text': 'Tôi không biết'}]},
        {'parts': [{'text': 'Cái gì'}]},
        {'parts': [{'text': 'Hả'}]},
        {'parts': [{'text': 'Không hiểu'}]},
        {'parts': [{'text': 'Câu hỏi khác'}]}
    ],
    'messages': [
        {'text': {'text': ['intent: fallback']}}
    ]
}
try:
    response = client.create_intent(parent=parent, intent=fallback_intent)
    print(f'Created fallback intent: {response.display_name}')
    time.sleep(2)
except Exception as e:
    print(f'Error creating fallback intent: {e}')
    time.sleep(10)