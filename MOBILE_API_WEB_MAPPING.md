# PROMPT: MAPPING TOÀN BỘ API + WEB SANG MOBILE VÀ HOÀN THIỆN CHỨC NĂNG 4 — TÌM VIỆC VÀ ỨNG TUYỂN

## I. BỐI CẢNH PROJECT

Tôi đang phát triển hệ thống KET NOI FREELANCER SV với ba thành phần chính:

1. `FreelancerStudent.API`: Backend xử lý nghiệp vụ, xác thực, phân quyền và truy cập database SQL Server.
2. `FreelancerStudent.Web`: Website hiện tại, đã triển khai một số chức năng nghiệp vụ và sử dụng API để xử lý dữ liệu.
3. `Mobile`: Ứng dụng Flutter dành cho người dùng trên điện thoại.

Tôi cần hoàn thiện ứng dụng Mobile bằng cách mapping toàn bộ chức năng phù hợp từ API và Web sang Mobile, ưu tiên tái sử dụng API hiện có và bảo đảm dữ liệu giữa Web và Mobile được đồng bộ.

Đặc biệt, Mobile bắt buộc phải triển khai đầy đủ **Chức năng 4 — Tìm việc và ứng tuyển**, với các yêu cầu nghiệp vụ được quy định trong phần III của prompt này.

## II. MỤC TIÊU TỔNG THỂ

Thực hiện các công việc sau:

- Khảo sát toàn bộ source code API, Web và Mobile.
- Kiểm kê toàn bộ chức năng nghiệp vụ đang tồn tại trong API và Web.
- Mapping từng chức năng với màn hình, model, service và luồng xử lý tương ứng trên Mobile.
- Xác định chức năng đã hoàn thiện, chưa có, mới có giao diện hoặc chưa kết nối API.
- Xác định các API hiện có có thể tái sử dụng.
- Xác định chính xác những chức năng bắt buộc của Chức năng 4 còn thiếu trên Mobile.
- Bổ sung và hoàn thiện source code Mobile.
- Chỉ bổ sung hoặc mở rộng Backend khi đã xác minh API hiện tại không đáp ứng được nghiệp vụ cần thiết.
- Kiểm thử tích hợp từ Mobile đến API và database thông qua Backend.
- Lập báo cáo chi tiết về những phần đã triển khai và kết quả kiểm thử.

**Không được dừng lại ở việc phân tích hoặc tạo kế hoạch. Sau khi hoàn thành bước khảo sát, hãy tiếp tục triển khai thực tế trong phạm vi được phép.**

## III. CÁC CHỨC NĂNG BẮT BUỘC CỦA CHỨC NĂNG 4 — TÌM VIỆC VÀ ỨNG TUYỂN

Đây là nhóm chức năng ưu tiên bắt buộc phải hoàn thiện trên Mobile.

### 1. Tạo hồ sơ năng lực và cập nhật kỹ năng

Mobile phải cho phép Freelancer Sinh viên:

- Xem hồ sơ năng lực hiện tại.
- Tạo hồ sơ năng lực nếu nghiệp vụ hiện tại hỗ trợ.
- Cập nhật thông tin cá nhân và thông tin chuyên môn.
- Quản lý chuyên ngành, kỹ năng và các thông tin liên quan đến năng lực.
- Thêm, xem, cập nhật hoặc xóa kỹ năng theo quyền và nghiệp vụ hiện có.
- Quản lý portfolio hoặc minh chứng năng lực nếu hệ thống đã hỗ trợ.
- Kiểm tra và hiển thị dữ liệu hồ sơ được lưu thực tế trên Backend.
- Hiển thị lỗi validation khi dữ liệu không hợp lệ.

Hãy đối chiếu các entity, DTO, API và màn hình Web liên quan đến `FreelancerStudents`, `ChuyenNganh`, kỹ năng, portfolio và các bảng liên quan trong database hiện tại.

Không tự suy đoán tên trường hoặc endpoint. Phải xác minh trong source code.

### 2. Tìm kiếm công việc

Mobile phải cho phép Freelancer:

- Xem danh sách công việc đang được phép hiển thị.
- Tìm kiếm công việc theo từ khóa.
- Lọc theo các tiêu chí mà API hiện tại hỗ trợ, chẳng hạn kỹ năng, chuyên ngành, ngân sách, thời hạn hoặc trạng thái tuyển dụng.
- Xem chi tiết công việc.
- Xem thông tin Nhà tuyển dụng và các điều kiện công việc được API cung cấp.
- Phân trang hoặc tải thêm dữ liệu nếu API hỗ trợ.
- Hiển thị trạng thái loading, danh sách rỗng và lỗi tải dữ liệu.

Không tự triển khai bộ lọc trên giao diện nếu nó làm sai lệch kết quả hoặc mâu thuẫn với logic tìm kiếm trên Backend.

Nếu API hiện tại chỉ hỗ trợ một phần tiêu chí tìm kiếm, hãy ghi rõ giới hạn và tận dụng tối đa chức năng hiện có.

### 3. Nhận gợi ý công việc phù hợp

Mobile phải có khu vực gợi ý công việc dành cho Freelancer.

Yêu cầu:

- Xác minh Web/API hiện tại có chức năng gợi ý công việc hay không.
- Nếu đã có API gợi ý, sử dụng API đó và ánh xạ kết quả lên Mobile.
- Hiển thị danh sách công việc được gợi ý theo kết quả Backend trả về.
- Nếu hệ thống có giải thích lý do phù hợp, hãy hiển thị thông tin đó.
- Cho phép mở chi tiết công việc từ danh sách gợi ý.

Không tự tạo thuật toán hoặc dữ liệu gợi ý giả trên Mobile để thay thế chức năng Backend.

Nếu API chưa có chức năng gợi ý, hãy báo cáo rõ. Chỉ đề xuất bổ sung API hoặc logic Backend tối thiểu khi thật sự cần thiết và không được giả định rằng chức năng đã tồn tại.

### 4. Lưu công việc yêu thích

Mobile phải cho phép Freelancer:

- Lưu một công việc để xem lại sau.
- Bỏ lưu công việc.
- Xem danh sách công việc đã lưu.
- Hiển thị chính xác trạng thái đã lưu hoặc chưa lưu.
- Đồng bộ trạng thái lưu giữa các màn hình.
- Duy trì trạng thái lưu sau khi đăng xuất hoặc đăng nhập lại theo đúng cơ chế lưu trữ của hệ thống.

Hãy kiểm tra xem API và database đã có cơ chế lưu công việc yêu thích hay chưa.

Nếu đã có, tái sử dụng API đó.

Nếu chưa có, không giả lập việc lưu trên giao diện rồi coi như đã lưu thành công. Hãy báo cáo phần Backend hoặc cơ chế lưu trữ còn thiếu và đề xuất phương án triển khai tối thiểu phù hợp với kiến trúc hiện tại.

### 5. Ứng tuyển công việc

Mobile phải cho phép Freelancer:

- Ứng tuyển vào công việc đủ điều kiện.
- Kiểm tra dữ liệu bắt buộc trước khi gửi.
- Gửi thông tin ứng tuyển theo DTO thực tế của API.
- Hiển thị kết quả thành công hoặc thất bại.
- Ngăn gửi lặp yêu cầu nếu nghiệp vụ hiện tại không cho phép ứng tuyển nhiều lần.
- Xử lý trường hợp công việc đã đóng, hết hạn hoặc không còn nhận ứng tuyển.
- Xử lý trường hợp Freelancer đã ứng tuyển.
- Hiển thị trạng thái ứng tuyển mới nhất.
- Điều hướng phù hợp sau khi ứng tuyển thành công.

Đối chiếu chính xác nghiệp vụ liên quan đến `JobPost`, `UngTuyen`, Freelancer, Nhà tuyển dụng và các API tương ứng trong project.

Không tự quyết định trạng thái ứng tuyển hoặc ghi nhận ứng tuyển thành công trên Mobile. Backend phải là nơi xác nhận kết quả.

### 6. Theo dõi trạng thái ứng tuyển

Mobile phải cung cấp khu vực quản lý các công việc đã ứng tuyển.

Freelancer có thể:

- Xem danh sách công việc đã ứng tuyển.
- Xem thông tin công việc tương ứng.
- Xem ngày ứng tuyển nếu dữ liệu có hỗ trợ.
- Theo dõi trạng thái hiện tại của từng hồ sơ ứng tuyển.
- Xem chi tiết trạng thái hoặc thông tin phản hồi nếu hệ thống có cung cấp.
- Tải lại dữ liệu để nhận trạng thái mới nhất.
- Phân biệt các trạng thái thực tế mà Backend trả về.

Không tự tạo enum trạng thái mới nếu API đã có quy ước trạng thái riêng.

Phải xác minh chính xác các trạng thái trong model, DTO, API và database. Không tự mặc định một bộ trạng thái khi chưa kiểm tra source code.

### 7. Chat với Nhà tuyển dụng

Mobile phải hỗ trợ trao đổi giữa Freelancer và Nhà tuyển dụng trong phạm vi hệ thống hiện có.

Yêu cầu:

- Xem danh sách cuộc trò chuyện nếu API hỗ trợ.
- Mở cuộc trò chuyện tương ứng với người dùng hoặc công việc/hợp đồng.
- Tải lịch sử tin nhắn.
- Gửi tin nhắn.
- Hiển thị người gửi, nội dung và thời gian gửi theo dữ liệu API.
- Hiển thị trạng thái gửi thành công hoặc thất bại.
- Tải hoặc nhận tin nhắn mới theo cơ chế thực tế của hệ thống.
- Kiểm tra quyền truy cập phòng chat.
- Không cho phép truy cập cuộc trò chuyện của người dùng khác.

Kiểm tra cơ chế chat hiện có, bao gồm API, bảng `PhongChat`, các model tin nhắn nếu có, cơ chế realtime, SignalR hoặc phương thức tải dữ liệu hiện tại.

Chỉ sử dụng cơ chế mà project thực sự hỗ trợ. Không tự tạo một hệ thống chat riêng hoặc tuyên bố có realtime nếu chưa triển khai.

### 8. Xem hợp đồng

Mobile phải cho phép Freelancer:

- Xem danh sách hợp đồng thuộc tài khoản.
- Xem chi tiết hợp đồng.
- Xem các bên tham gia.
- Xem phạm vi công việc, giá trị hợp đồng và thời hạn nếu có dữ liệu.
- Theo dõi trạng thái hợp đồng.
- Xem thông tin ký quỹ hoặc thanh toán liên quan nếu API cung cấp.
- Điều hướng đến các chức năng tiến độ và bàn giao tương ứng.

Đối chiếu các bảng, entity, DTO, API và giao diện Web hiện tại liên quan đến `HopDong`, `UngThue` và các thành phần có liên quan.

Không tự tạo hợp đồng trên Mobile nếu nghiệp vụ hiện tại yêu cầu hợp đồng phải được hình thành thông qua quy trình thuê hoặc thỏa thuận ở Backend.

### 9. Cập nhật tiến độ dự án

Mobile phải cho phép Freelancer theo dõi và cập nhật tiến độ hợp đồng đang thực hiện nếu Backend đã hỗ trợ nghiệp vụ này.

Yêu cầu:

- Xem danh sách công việc hoặc task thuộc hợp đồng.
- Xem mô tả, deadline và trạng thái từng task nếu dữ liệu có hỗ trợ.
- Thêm, chỉnh sửa hoặc xóa task theo đúng quyền và quy tắc nghiệp vụ.
- Cập nhật trạng thái task.
- Đính kèm minh chứng tiến độ nếu hệ thống hỗ trợ.
- Xem tỷ lệ hoàn thành dự án nếu Backend cung cấp hoặc có thể tính chính xác từ dữ liệu hiện có.
- Kiểm tra điều kiện trạng thái hợp đồng trước khi cho phép chỉnh sửa.
- Không hiển thị dữ liệu tiến độ thuộc hợp đồng khác.

Hãy kiểm tra cấu trúc hiện tại của `Task_CongViec`, `HopDong` và các bảng liên quan trước khi triển khai.

Không tự ý thêm bảng hoặc cột database để phục vụ chức năng này.

Nếu chức năng chưa được hỗ trợ ở Backend, hãy ghi rõ phần còn thiếu và đề xuất phương án phù hợp với kiến trúc hiện tại.

### 10. Nộp sản phẩm bàn giao

Mobile phải cho phép Freelancer thực hiện các thao tác bàn giao sản phẩm theo nghiệp vụ đã triển khai trên Web/API.

Yêu cầu:

- Xem thông tin và điều kiện bàn giao.
- Gửi sản phẩm bàn giao theo API hiện có.
- Đính kèm tệp hoặc đường dẫn sản phẩm nếu hệ thống hỗ trợ.
- Gửi mô tả hoặc ghi chú bàn giao nếu DTO cho phép.
- Theo dõi trạng thái bàn giao.
- Xem phản hồi của Nhà tuyển dụng nếu API hỗ trợ.
- Xử lý lỗi upload và lỗi nghiệp vụ.
- Không đánh dấu bàn giao thành công khi Backend chưa xác nhận.

Kiểm tra các model, API và bảng liên quan đến `BanGiao_SanPham`, `HopDong` và các nghiệp vụ bàn giao hiện có.

Không tự ý thay đổi logic bàn giao, nghiệm thu hoặc giải ngân.

### 11. Theo dõi thanh toán

Mobile phải cung cấp thông tin thanh toán liên quan đến Freelancer theo API hiện có.

Yêu cầu:

- Xem số dư ví khả dụng và số dư đóng băng nếu API hỗ trợ.
- Xem lịch sử giao dịch.
- Xem các khoản thanh toán liên quan đến hợp đồng.
- Theo dõi trạng thái ký quỹ, thanh toán hoặc giải ngân nếu API cung cấp.
- Xem chi tiết giao dịch nếu có.
- Hiển thị trạng thái giao dịch theo dữ liệu Backend.

Đối chiếu các bảng và API hiện có liên quan đến `Wallet`, `LichSuGiaoDich`, `HopDong`, `YeuCauNapTien`, `YeuCauRutTien` và các thành phần tài chính khác.

Không tự tính số dư để thay thế số dư do Backend trả về.

Không tự cập nhật trạng thái giao dịch, số dư ví, số tiền ký quỹ hoặc trạng thái giải ngân từ Mobile.

Nếu cần thực hiện giao dịch, phải sử dụng đúng API và quy trình nghiệp vụ đã được triển khai.

### 12. Gửi phản ánh hoặc khiếu nại

Mobile phải cho phép Freelancer gửi phản ánh liên quan đến hợp đồng hoặc quá trình làm việc nếu API hiện tại hỗ trợ.

Yêu cầu:

- Xác định các loại phản ánh được hệ thống hỗ trợ.
- Chọn đối tượng hoặc hợp đồng liên quan nếu nghiệp vụ yêu cầu.
- Nhập nội dung phản ánh.
- Đính kèm bằng chứng nếu có hỗ trợ.
- Gửi phản ánh thông qua API.
- Theo dõi trạng thái xử lý nếu API cung cấp.
- Xem thông tin phản hồi nếu hệ thống hỗ trợ.
- Kiểm tra quyền truy cập và quyền thao tác.

Đối chiếu các thành phần liên quan đến `TranhChap`, `BANGCHUNG_TRANHCHAP` và các API thực tế.

Không tự tạo trạng thái khiếu nại hoặc cơ chế giải quyết tranh chấp mới nếu Backend chưa hỗ trợ.

### 13. Đánh giá Nhà tuyển dụng

Mobile phải cho phép Freelancer đánh giá Nhà tuyển dụng sau khi đáp ứng điều kiện nghiệp vụ hiện có.

Yêu cầu:

- Hiển thị chức năng đánh giá khi hợp đồng hoặc giao dịch đáp ứng điều kiện.
- Nhập điểm đánh giá theo thang điểm thực tế của hệ thống.
- Nhập nhận xét nếu API hỗ trợ.
- Gửi đánh giá bằng API hiện có.
- Hiển thị kết quả lưu thành công hoặc thất bại.
- Hiển thị đánh giá đã gửi nếu có API đọc dữ liệu.
- Ngăn đánh giá trùng hoặc đánh giá sai đối tượng theo quy tắc Backend.

Phải xác minh API, model và database hiện có về đánh giá trước khi triển khai.

Không tự suy đoán điều kiện đánh giá, thang điểm hoặc trạng thái hợp đồng đủ điều kiện.

## IV. QUY TRÌNH MAPPING BẮT BUỘC

### Bước 1 — Khảo sát API

Đọc toàn bộ API Controller, Service, Repository, DTO, entity và cấu hình liên quan.

Lập danh sách endpoint thực tế, bao gồm:

- HTTP method.
- Route.
- Controller và action.
- Request DTO.
- Response DTO.
- Authentication và authorization.
- Quy tắc nghiệp vụ.
- Các trường hợp lỗi.
- Các bảng hoặc entity được truy cập.

Không chỉ tìm endpoint bằng tên. Phải đọc code xử lý để xác minh chức năng thực tế.

### Bước 2 — Khảo sát Web

Đọc toàn bộ Web Controller, Web Service, ViewModel, View và luồng thao tác tương ứng.

Với mỗi chức năng:

- Xác định màn hình.
- Xác định hành động của người dùng.
- Xác định API được gọi.
- Xác định dữ liệu đầu vào và đầu ra.
- Xác định điều kiện nghiệp vụ.
- Xác định cách xử lý lỗi và cập nhật giao diện.

Dùng Web để hiểu chức năng và đối chiếu hành vi, không sao chép máy móc logic giao diện sang Flutter.

### Bước 3 — Khảo sát Mobile

Kiểm tra các model, service, screen, widget, route và cơ chế quản lý trạng thái hiện có.

Xác định từng chức năng thuộc một trong các nhóm:

- Đã hoàn thiện và sử dụng API thật.
- Có giao diện nhưng chưa gọi API.
- Đã gọi API nhưng chưa hoàn thiện.
- Chưa có trên Mobile.
- Không thể hoàn thiện vì API chưa hỗ trợ.
- Không áp dụng cho Mobile.

### Bước 4 — Lập bảng mapping

Tạo tài liệu `MOBILE_API_WEB_MAPPING.md` trong thư mục gốc của project nếu chưa có.

Bảng mapping tối thiểu phải gồm:

| Nhóm | Chức năng | Web | API hiện có | Mobile hiện tại | Phần thiếu | Hướng xử lý | Trạng thái |
|---|---|---|---|---|---|---|---|

Mỗi chức năng phải được đối chiếu với endpoint, DTO và file source code thực tế.

Bảng mapping phải bao phủ toàn bộ chức năng phù hợp được tìm thấy trong API và Web, không chỉ 13 chức năng trong phần III.

Đồng thời, tạo danh sách riêng các chức năng bắt buộc của Chức năng 4 và đánh dấu rõ trạng thái hoàn thành của từng chức năng.

### Bước 5 — Lập danh sách file cần thay đổi

Trước khi triển khai, xác định:

- File cần tạo mới.
- File cần bổ sung.
- File cần chỉnh sửa để kết nối API.
- Model cần bổ sung.
- Service cần bổ sung.
- Màn hình cần hoàn thiện.
- Route cần tích hợp.
- API còn thiếu.
- Các dependency giữa chức năng.

Ưu tiên sửa đổi tối thiểu và tái sử dụng code hiện có.

### Bước 6 — Triển khai thực tế

Sau khi hoàn thành mapping, tiếp tục triển khai trực tiếp trên source code.

Ưu tiên theo thứ tự:

1. Authentication và API client.
2. Hồ sơ năng lực và kỹ năng.
3. Tìm kiếm, lọc và chi tiết công việc.
4. Gợi ý công việc và lưu công việc.
5. Ứng tuyển và theo dõi trạng thái ứng tuyển.
6. Chat với Nhà tuyển dụng.
7. Xem hợp đồng.
8. Cập nhật tiến độ.
9. Nộp sản phẩm bàn giao.
10. Theo dõi thanh toán.
11. Gửi phản ánh và đánh giá Nhà tuyển dụng.
12. Các chức năng phù hợp khác được phát hiện trong API/Web.

Thứ tự trên là thứ tự ưu tiên triển khai, không phải giả định rằng mọi API đã tồn tại. Nếu một chức năng phụ thuộc API chưa có, hãy ghi nhận rõ và tiếp tục các chức năng độc lập khác.

## V. NGUYÊN TẮC KẾT NỐI API

1. Mobile chỉ giao tiếp với Backend thông qua HTTP API hoặc cơ chế kết nối mà project thực tế đang sử dụng.
2. Không kết nối trực tiếp Mobile đến SQL Server.
3. Không hardcode dữ liệu giả để thay thế API.
4. Không tự tạo endpoint theo phỏng đoán.
5. Không tự đổi tên trường JSON hoặc DTO của API.
6. Không tự thay đổi quy tắc nghiệp vụ.
7. Tái sử dụng HTTP client và cơ chế quản lý token hiện có nếu phù hợp.
8. Xử lý đúng các mã HTTP và response lỗi.
9. Kiểm tra trạng thái loading, lỗi mạng, dữ liệu rỗng và validation.
10. Sau thao tác tạo, sửa hoặc xóa, phải cập nhật giao diện dựa trên kết quả thực tế từ Backend.
11. Với thao tác tài chính, hợp đồng và bàn giao, phải chờ kết quả xác nhận từ Backend.
12. Cấu hình Base URL phù hợp với môi trường chạy thực tế, bao gồm emulator và thiết bị thật khi cần.

## VI. RÀNG BUỘC DATABASE VÀ NGHIỆP VỤ

Không tự ý:

- Thêm hoặc xóa bảng.
- Thêm hoặc xóa cột.
- Đổi tên bảng hoặc cột.
- Thay đổi khóa chính, khóa ngoại hoặc constraint.
- Chạy migration làm thay đổi schema.
- Xóa hoặc sửa dữ liệu nghiệp vụ hiện có.
- Tạo một hệ thống lưu trữ riêng trên Mobile để thay thế database chính.

Trước khi kết luận API còn thiếu, hãy kiểm tra tất cả controller, service và endpoint liên quan. Tránh tạo API trùng lặp với API đã tồn tại dưới tên khác.

Nếu API không hỗ trợ chức năng bắt buộc:

1. Nêu rõ chức năng đang thiếu.
2. Chỉ ra các file và endpoint đã kiểm tra.
3. Xác định dữ liệu hiện tại có đủ để triển khai hay không.
4. Đề xuất phương án Backend tối thiểu.
5. Không tự ý thay đổi database schema.
6. Chỉ triển khai phần Backend bổ sung khi có thể thực hiện an toàn và phù hợp với kiến trúc hiện tại.
7. Không giả lập chức năng đã hoàn thiện khi chưa có cơ chế lưu trữ hoặc xử lý thực tế.

## VII. KIỂM THỬ

Kiểm thử từng chức năng sau khi triển khai.

### 1. Hồ sơ và kỹ năng

- Tải hồ sơ hiện tại.
- Cập nhật thông tin.
- Thêm hoặc cập nhật kỹ năng.
- Kiểm tra dữ liệu sau khi tải lại.
- Kiểm tra lỗi validation.

### 2. Tìm việc và ứng tuyển

- Tìm kiếm theo từ khóa.
- Lọc và mở chi tiết công việc.
- Tải danh sách gợi ý.
- Lưu và bỏ lưu công việc.
- Ứng tuyển thành công.
- Kiểm tra ứng tuyển trùng.
- Theo dõi trạng thái ứng tuyển sau khi tải lại.

### 3. Chat và hợp đồng

- Tải danh sách chat và lịch sử tin nhắn.
- Gửi tin nhắn.
- Kiểm tra quyền truy cập.
- Tải danh sách và chi tiết hợp đồng.
- Kiểm tra các trạng thái hợp đồng thực tế.

### 4. Tiến độ và bàn giao

- Tải danh sách công việc thuộc hợp đồng.
- Cập nhật tiến độ nếu API hỗ trợ.
- Gửi sản phẩm bàn giao.
- Kiểm tra trạng thái sau khi Backend xử lý.
- Kiểm tra quyền và điều kiện hợp đồng.

### 5. Thanh toán, phản ánh và đánh giá

- Tải số dư và lịch sử giao dịch.
- Theo dõi trạng thái giao dịch.
- Gửi phản ánh.
- Tải trạng thái xử lý nếu có.
- Gửi đánh giá khi đủ điều kiện.
- Kiểm tra trường hợp không đủ quyền hoặc không đủ điều kiện nghiệp vụ.

### 6. Kiểm thử tổng thể

- Build Flutter.
- Chạy phân tích Dart.
- Kiểm tra API hoạt động.
- Kiểm tra dữ liệu thực tế.
- Kiểm tra điều hướng.
- Kiểm tra các lỗi phát sinh từ thay đổi mới.
- Đảm bảo chức năng cũ không bị ảnh hưởng.

Nếu không thể chạy kiểm thử do thiếu cấu hình hoặc môi trường, hãy báo cáo rõ thay vì tuyên bố đã kiểm thử thành công.

## VIII. QUẢN LÝ THAY ĐỔI SOURCE CODE

- Không xóa source code đang hoạt động.
- Không ghi đè toàn bộ file chỉ để bổ sung một chức năng.
- Không viết lại kiến trúc hiện tại nếu không cần thiết.
- Không tự ý nâng cấp Flutter, Dart hoặc dependency.
- Không thay đổi API hoặc Web nếu chưa xác minh nhu cầu.
- Không sửa database schema.
- Không thay đổi logic nghiệp vụ hiện có chỉ để làm cho Mobile dễ triển khai.
- Bảo toàn các thay đổi local hiện tại.
- Nếu phát hiện xung đột không thể giải quyết an toàn, hãy báo cáo rõ.

API và Web được sử dụng làm nguồn tham chiếu và chỉ được chỉnh sửa khi thực sự cần thiết để hỗ trợ chức năng chưa được Backend đáp ứng. Mobile là thành phần được ưu tiên triển khai.

## IX. BÁO CÁO CUỐI CÙNG

Sau khi hoàn thành, cung cấp các nội dung:

### A. Tổng quan mapping

- Tổng số nhóm nghiệp vụ đã kiểm tra.
- Tổng số chức năng được phát hiện.
- Số chức năng đã hoàn thiện trên Mobile.
- Số chức năng chỉ có giao diện.
- Số chức năng đã có API nhưng Mobile chưa tích hợp.
- Số chức năng còn thiếu.
- Số chức năng bị chặn do API chưa hỗ trợ.

### B. Kết quả Chức năng 4

Báo cáo riêng 13 chức năng bắt buộc trong phần III, với trạng thái:

- Hoàn thành và đã kiểm thử.
- Đã triển khai nhưng chưa kiểm thử đầu cuối.
- Chưa hoàn thành.
- Bị chặn do API hoặc dependency còn thiếu.

Mỗi trạng thái phải có bằng chứng cụ thể từ file code, endpoint hoặc kết quả kiểm thử.

### C. Danh sách file thay đổi

Với từng file, ghi rõ:

- Đường dẫn.
- Mục đích thay đổi.
- Nội dung được bổ sung hoặc chỉnh sửa.
- API liên quan.
- Chức năng được hoàn thiện.
- Kết quả kiểm thử.

### D. API còn thiếu

Liệt kê các chức năng chưa được Backend hỗ trợ, kèm lý do và phương án xử lý tối thiểu.

### E. Kết quả kiểm thử

Ghi rõ kết quả build, phân tích code và kiểm thử API/Mobile thực tế. Phân biệt lỗi có sẵn và lỗi phát sinh từ thay đổi mới.

### F. Trạng thái hoàn thiện cuối cùng

Không đánh dấu toàn bộ Mobile là hoàn thành nếu còn chức năng bắt buộc chưa hoạt động.

## X. CHỈ THỊ CUỐI CÙNG

Hãy thực hiện trực tiếp trên source code hiện có của:

- `FreelancerStudent.API`
- `FreelancerStudent.Web`
- `Mobile` (Flutter)

Thực hiện theo quy trình:

**Khảo sát source code → Kiểm kê chức năng → Mapping API/Web/Mobile → Xác định thiếu sót → Lập thứ tự triển khai → Bổ sung code → Kết nối API thật → Kiểm thử → Báo cáo.**

Không chỉ đưa ra kế hoạch. Hãy tiếp tục triển khai thực tế sau khi hoàn thành mapping.

Mục tiêu là hoàn thiện Mobile bằng cách tái sử dụng API hiện có, bảo đảm dữ liệu và nghiệp vụ thống nhất giữa Web và Mobile, đồng thời hoàn thành đầy đủ các yêu cầu bắt buộc của Chức năng 4.

Nếu chưa xác định được đường dẫn project hoặc có thao tác cần thay đổi phạm vi nghiệp vụ, hãy hỏi tôi trước khi thực hiện. Không tự suy đoán hoặc tạo ra các chức năng đã tồn tại dưới tên khác.

---

# KẾT QUẢ MAPPING TOÀN BỘ API + WEB SANG MOBILE

## I. BẢNG MAPPING TỔNG THỂ

| Nhóm | Chức năng | Web | API hiện có | Mobile hiện tại | Phần thiếu | Hướng xử lý | Trạng thái |
|---|---|---|---|---|---|---|---|
| **1. Xác thực** | Đăng nhập | `Auth/DangNhap` | `POST api/Auth/DangNhap` | `LoginScreen`, `AuthService`, `AuthViewModel` | Không | Sử dụng token lưu trữ an toàn |  Hoàn thành |
| **1. Xác thực** | Đăng ký tài khoản | `Auth/DangKy` | `POST api/Auth/dangky` | `RegisterScreen`, `AuthService` | Không | Đã bổ sung số điện thoại |  Hoàn thành |
| **1. Xác thực** | Đổi mật khẩu | `Auth/DoiMatKhau` | `PUT api/Auth/DoiMatKhau` | `ChangePasswordScreen`, `UserService` | Không | Kết nối API đổi mật khẩu |  Hoàn thành |
| **2. Hồ sơ cá nhân** | Xem thông tin hồ sơ | `FreelancerStudent/HoSo` | `GET api/FreelancerStudent/Profile/{id}` | `ProfileScreen`, `FreelancerProfileViewModel` | Không | Tải dữ liệu hồ sơ thật |  Hoàn thành |
| **2. Hồ sơ cá nhân** | Cập nhật hồ sơ & kỹ năng | `FreelancerStudent/CapNhatHoSo` | `PUT api/FreelancerStudent/CapNhatHoSo` | `EditProfileScreen`, `FreelancerProfileService` | Không | Cập nhật thông tin & kỹ năng |  Hoàn thành |
| **2. Hồ sơ cá nhân** | Thêm dự án Portfolio | `FreelancerStudent/HoSo` | `POST api/Portfolio/projects` | `ProfileScreen` (Dialog thêm dự án) | Không | Kết nối API Portfolio |  Hoàn thành |
| **3. Việc làm** | Danh sách việc làm | `JobPost/Index` | `GET api/JobPost/DanhSachJobPost` | `HomeScreen` (Tab Việc làm), `JobViewModel` | Không | Lọc phía client vì API chưa hỗ trợ query lọc |  Hoàn thành |
| **3. Việc làm** | Chi tiết việc làm | `JobPost/Index` (Modal) | `GET api/JobPost/ChiTietJobPost/{maJob}` | `JobDetailScreen`, `JobService` | Không | Tải chi tiết công việc từ API |  Hoàn thành |
| **3. Việc làm** | Gợi ý việc làm | Tự động match hồ sơ | `GET api/JobPost/GoiYChoFreelancer/{maUser}` | `HomeScreen` (Khối Gợi ý việc làm, Badge % phù hợp), `JobService`, `JobViewModel` | Không | Thuật toán Server-side Matching kỹ năng & chuyên ngành |  Hoàn thành |
| **3. Việc làm** | Lưu việc làm yêu thích | Chưa có | Chưa có bảng & API lưu việc làm | Không | Cần API & bảng `JobYeuThich` | ⚠️ Chưa có API |
| **4. Ứng tuyển** | Nộp đơn ứng tuyển | `JobPost/Index` (Modal) | `POST api/JobPost/UngTuyen` | `ApplyJobBottomSheet`, `ApplicationService` | Không | Gửi hồ sơ ứng tuyển |  Hoàn thành |
| **4. Ứng tuyển** | Quản lý đơn ứng tuyển | `JobPost/DanhSachUngTuyen` | `GET api/JobPost/DanhSachUngTuyen/{maUser}` | `MyApplicationsScreen`, `ApplicationViewModel` | Không | Hiển thị trạng thái đơn ứng tuyển thật |  Hoàn thành |
| **5. Hợp đồng** | Danh sách hợp đồng | `HopDong/Index` | `GET api/HopDong/danh-sach-freelancer/{maFS}` | `HopDongListScreen`, `HopDongViewModel` | Không | Hiển thị danh sách hợp đồng |  Hoàn thành |
| **5. Hợp đồng** | Chi tiết hợp đồng | `HopDong/ChiTiet` | `GET api/HopDong/chi-tiet/{maHD}` | `HopDongDetailScreen`, `HopDongViewModel` | Không | Xem điều khoản, chi phí ký quỹ |  Hoàn thành |
| **5. Hợp đồng** | Xác nhận / Từ chối HĐ | `HopDong/ChiTiet` | `POST api/HopDong/xac-nhan` | `HopDongDetailScreen` (Nút Xác nhận / Từ chối) | Không | Gửi lý do từ chối nếu không đồng ý |  Hoàn thành |
| **6. Tiến độ dự án** | Xem tiến độ hợp đồng | `HopDong/ChiTiet` (Tab tiến độ) | `GET api/TaskCongViec/hop-dong/{maHD}` | `TienDoScreen`, `TaskViewModel` | Không | Xem tỷ lệ hoàn thành, danh sách task |  Hoàn thành |
| **6. Tiến độ dự án** | Thêm / Sửa / Xóa task | `HopDong/ChiTiet` | `POST / PUT / DELETE api/TaskCongViec` | `TienDoScreen` (Form dialog) | Không | Kiểm tra quyền `coQuyenCapNhat` |  Hoàn thành |
| **6. Tiến độ dự án** | Cập nhật trạng thái task | `HopDong/ChiTiet` | `PATCH api/TaskCongViec/{maTask}/trang-thai` | `TienDoScreen` (Dropdown trạng thái) | Không | Cập nhật trạng thái đầu việc |  Hoàn thành |
| **7. Chat** | Danh sách phòng chat | Web Chat | `GET api/Chat/DanhSachPhong/{maUser}` | `ChatListScreen`, `ChatViewModel` | Không | Hiển thị phòng chat, tin nhắn chưa đọc |  Hoàn thành |
| **7. Chat** | Nhắn tin thời gian thực | Chat Hub | `POST api/Chat/GuiTinNhan`, Hub SignalR `/chatHub` | `ChatDetailScreen`, `ChatSignalrService` | Không | Gửi nhận tin nhắn realtime + REST |  Hoàn thành |
| **8. Tài chính** | Xem số dư ví | `Wallet/Index` | `GET api/Wallet/LayViTheoUser/{maUser}` | `WalletScreen`, `WalletViewModel` | Không | Hiển thị số dư khả dụng & đóng băng |  Hoàn thành |
| **8. Tài chính** | Nạp tiền vào ví | `Wallet/Index` | `POST api/GiaoDichNapTien/TaoGiaoDich/{maUser}` | `DepositScreen`, `WalletService` | Không | Tạo đơn SePay (tối thiểu 100k, bội số 50k) |  Hoàn thành |
| **8. Tài chính** | Lịch sử giao dịch | `Wallet/Index` | Chưa có API GET lịch sử giao dịch | `TransactionHistoryScreen` | API GET | Báo trạng thái trung thực, không fake dữ liệu | ⚠️ Thiếu API GET |
| **9. Bàn giao** | Nộp sản phẩm bàn giao | Chưa có | Chưa có entity & API bàn giao | Không | Module bàn giao | ⚠️ Chưa có API |
| **10. Khiếu nại** | Gửi phản ánh/khiếu nại | Chưa có | Chưa có API giải quyết tranh chấp | Không | Module tranh chấp | ⚠️ Chưa có API |
| **11. Đánh giá** | Đánh giá Nhà tuyển dụng | Chỉ hiển thị sao | Chưa có API & bảng đánh giá chi tiết | Không | Bảng `DanhGia` | ⚠️ Chưa có API |