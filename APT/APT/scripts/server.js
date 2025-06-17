const express = require('express');
const fetch = require('node-fetch');
const app = express();
const port = 3000;

app.use(express.json());
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept');
    next();
});

app.post('/chat', async (req, res) => {
    const userInput = req.body.message;
    try {
        const response = await fetch('https://dialogflow.googleapis.com/v2/projects/apartmentchatbot-458709/agent/sessions/123456:detectIntent', {
            method: 'POST',
            headers: {
                'Authorization': 'ya29.c.c0ASRK0GYvVY9--XoqSf7nIWNviLrOgxmMmM1oDmTJWO5aZuhUzTAqGzDP6JZqGFmX8r2dcx43b6zWCIsRZ8pOLpGu6UckqPMcfE0Xkm4-BvBRLJ-BGEYB1vP3ujA7BKKXbbBQgpib7CvTg4K0Ur9d3fwzlTIBrnSylG7mKkROLAxwzBOzP-QgjNxDLbvYrjyZt78lBpnpq9-vyWmstVfUlOSuojpdC7QAA2G7CdOr45yS3JM8P7O7im0ao9sp4rDIsXg5Tt6EMJ7kOnze1zwC93vfEmDYRAGbTKMMNpkF3rcTt0eXtAnlffSZStG-LIXlcOB2sG6WgnBZBaiB3IGmJjPnfR3DKwGGLt3w0JCBmpJyhzHcxz1ML381Ck6Y8fc0jwYOwdpcu865m3V2-pkxSncm0xg49m_dvt2pI10uwrmBm7Qp96f2IRgq2ciJdxnrpfb30e-qnRjXZdxQS7a3XXw6IhwuMbmI4BunuvYcR85eOup4w_lU4zre3lgWae-535heBiM-hQbcpYJRlymeW83UpS-Iw902Fz5lrnX1bgOm6FMnkdJ_fyBZQYJ9k0x7Xymu8r_Wb4iRzsxrB23czUwXf0RY2o39YQFsrRd60dB5isFgc-kjvqXW3iuoYfm7Vo4YjMpFUaIlyVIkQvjgJogSczcMJgRbR9g8R8pwtdeb8pnX_I054eaRbrt0IwJYUJWbit033UeU6umS0WYOmpj3l5MalW9siZc5OrZvqkgm_Jxsh9u-ZxucBdau787cu5xQtbq-MYWIBoRx4huUScmx60Mg4kBb3-sn4RXZ6-zjR709cz6exxrOrrg9u7W4Rnt3zh_ZjOSbxOiOIR1Fu9YIFdRbp7IU75w1tUXZc-dFyXkZywuXYhJnOOxO8-1wdW6SjcBVavppZ_rdwnr_y9Zi6xvlkVavY2BW6xtR9s2svyV1f5vuhWRe4jmj1MI5cVhyZ95FcUq3a53MROu-3JbZ73aWvJW-dBcUqVVwUwv8e5n_Rd_gdqRya29.c.c0ASRK0GbIuPbahasUmZJaHmUak_P92Q6J4qaKJv4yFMEuywYucpyEgCLwpIzDdj70gfxcA_XCCSI3TJR0WqT3wNqidjh30rfsY3i7PNvbQRtnS2HYyVzbDLRRg5T5peFbRiS2uU8yc2R4sqXkBGHSvZMPNVWq9W61E5_2mFLNymLzunxzufRqkDKo_sPp993-lstyF1BR5zRuIfo45MpxtDfPN7vqBLRZV02ecXYNgN494-pSXX5qPAcoRM2x6eCAuvcnmSCK9wQFy1xoexX7TwFNBV6_No7j7keoqjlVy8tLGcR3JYJkkKIcH5S8WdwEmduFk2iG-17npMjYiYaHOD8bgKNxe5QC6yO7BVNEP08DKLoxu5l-wh2egQG387KIqv4ww7YenrIOf9pFqcUF4rlslq-q7O-7naO8WRriJW79pykWqioIZ4SiajtgSj4iUVYB8Xh6i4fbvXM7yjUXVZ-vwbUeeYeeag_ojnMq20pmn1yp2XxQqdFcXwFq5UqbR_utmjqyaehxfJg-gRjQnQ7zk82mhQzo79Jrlzgy1mymyw7R6ljiZnhnmSm127xmai8UFMix42Yh2dkXrbe8aZVwYsyYJjQcgxnv3YrXYFsbmy1tIjk2k74aB_6OrnVg2W9jqV3mYXzB7tro60hfMMFORFvw5d-surRkhh70wawg70mMv2qQIQr7lrxop6x8xJdShZl6QIrB92WqllsnaI36zpRv7q0YrX27xMVzq83Mgy6-ir2FU96aO5ii7XRfVlZ-BRMqnV-aBgznig4y9a1qB39m621i4aW9FsvliRx5cSQ2_fkJrMO0Uo5141-byVfr-SW8jF_FfYReVcSevdvikd_yUyJQi6pqVvwyZRl8IYScopgjkorQRpmVJuubyIW1SmiujWSgzivqMB-VMxe5grBp47RfMcQV5bSxhb9pqVaY3x1o0RuycSfawwdmMzm1jxYyFV4jYh52frMWZnaOF6pI_Xt-j4Jsqm0wzkSRwUZq0fht9wi',  // Thay bằng access token
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                queryInput: {
                    text: {
                        text: userInput,
                        languageCode: 'vi'
                    }
                }
            })
        });
        const data = await response.json();
        res.json({ reply: data.queryResult.fulfillmentText || 'Xin lỗi, tôi không hiểu.' });
    } catch (error) {
        res.status(500).json({ reply: `Có lỗi xảy ra: ${error.message}` });
    }
});

app.listen(port, () => console.log(`Server chạy tại http://localhost:${port}`));