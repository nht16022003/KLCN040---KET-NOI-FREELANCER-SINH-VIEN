# PROMPT: MAPPING VÀ HOÀN THIỆN TOÀN BỘ CHỨC NĂNG NHÀ TUYỂN DỤNG TỪ API + WEB SANG MOBILE

## I. BỐI CẢNH PROJECT

Tôi đang phát triển hệ thống KET NOI FREELANCER SV, bao gồm các thành phần:

- `FreelancerStudent.API`: Backend API xử lý nghiệp vụ, xác thực, phân quyền và truy cập database SQL Server.
- `FreelancerStudent.Web`: Website hiện tại, triển khai các chức năng nghiệp vụ và sử dụng API.
- `Mobile`: Ứng dụng Flutter dành cho người dùng trên điện thoại.

Hiện tại, phần Nhà tuyển dụng trên Mobile chưa được mapping đầy đủ từ API và Web.

Tôi cần bạn khảo sát toàn bộ source code của ba thành phần, xác định chính xác các chức năng Nhà tuyển dụng đã tồn tại, đối chiếu với Mobile và trực tiếp bổ sung những phần còn thiếu.

Mục tiêu cuối cùng là bảo đảm Mobile hỗ trợ đầy đủ các chức năng thực tế thuộc vai trò Nhà tuyển dụng đã được triển khai hoặc được yêu cầu chính thức trong project, sử dụng API phù hợp và duy trì tính nhất quán về dữ liệu, quyền hạn và nghiệp vụ.

## II. NGUYÊN TẮC XÁC ĐỊNH PHẠM VI

### 1. Source code thực tế là căn cứ chính

Không mặc định rằng mọi chức năng được liệt kê trong prompt, tài liệu gợi ý hoặc danh sách nghiệp vụ phổ biến đều đã tồn tại trong project.

Phải khảo sát thực tế:

- Controller, Service, Repository và DTO trong API.
- Controller, Web Service, ViewModel và View trong Web.
- Model, Service, Screen, Widget và luồng xử lý trong Mobile.
- Database schema hiện có khi cần xác minh quan hệ dữ liệu và nghiệp vụ.
- Tài liệu yêu cầu chính thức của project nếu có.

Chỉ xác định chức năng là đã tồn tại khi có đủ bằng chứng từ source code hoặc tài liệu nghiệp vụ chính thức.

Không coi việc tìm thấy tên bảng, entity, model, controller hoặc một file giao diện là bằng chứng đủ để kết luận chức năng đã hoàn thiện.

Phải đọc luồng xử lý thực tế để xác định chức năng có hoạt động hay không.

### 2. Danh sách gợi ý không phải danh sách chức năng mặc định

Các nhóm nghiệp vụ được đề cập trong prompt chỉ nhằm hỗ trợ quá trình khảo sát và tránh bỏ sót.

Đối với từng chức năng:

- Nếu xác minh được chức năng tồn tại trên Web hoặc API, hãy đưa vào bảng mapping.
- Nếu chức năng tồn tại nhưng Mobile chưa có, hãy xác định phần cần bổ sung.
- Nếu chỉ tồn tại một phần, hãy ghi rõ phần đã có và phần còn thiếu.
- Nếu không tìm thấy bằng chứng, không tự tạo chức năng và không đưa vào danh sách bắt buộc triển khai chỉ vì nó thường có trong một hệ thống tuyển dụng.
- Nếu có dấu hiệu tồn tại nhưng chưa đủ bằng chứng, đánh dấu cần xác minh.
- Nếu chức năng được yêu cầu rõ ràng trong tài liệu nghiệp vụ chính thức nhưng API chưa hỗ trợ, ghi nhận là yêu cầu chưa được đáp ứng và đề xuất phương án xử lý.
- Nếu chức năng không thuộc vai trò Nhà tuyển dụng hoặc không áp dụng cho Mobile, phải nêu rõ lý do trước khi loại khỏi phạm vi.

### 3. Phân biệt chức năng có sẵn và chức năng còn thiếu

Không kết luận một chức năng còn thiếu chỉ vì không tìm thấy màn hình có tên tương tự.

Phải kiểm tra xem chức năng đó có được tích hợp trong màn hình khác, dùng chung service hoặc sử dụng API thông qua một luồng xử lý khác hay không.

Chỉ xác định là thiếu khi đã đối chiếu đủ các thành phần liên quan.

## III. MỤC TIÊU MAPPING

Thực hiện ba nhiệm vụ chính:

1. Mapping toàn bộ chức năng thực tế thuộc vai trò Nhà tuyển dụng từ Web/API sang Mobile.
2. Xác định chính xác những phần Mobile chưa có, chưa kết nối API hoặc chưa hoạt động đúng.
3. Trực tiếp triển khai và kiểm thử các phần còn thiếu trong phạm vi đã xác minh.

Không chỉ lập danh sách hoặc đưa ra kế hoạch. Sau khi hoàn thành khảo sát và mapping, hãy tiếp tục chỉnh sửa source code thực tế.

## IV. GIAI ĐOẠN 1 — KHẢO SÁT TOÀN BỘ WEB

Đọc toàn bộ source code Web có liên quan đến vai trò Nhà tuyển dụng.

Kiểm tra các thành phần:

- Controllers.
- Web Services sử dụng `HttpClient`.
- ViewModels.
- Views.
- Các biểu mẫu và modal.
- Các nút thao tác.
- Các chức năng thêm, xem, sửa, xóa hoặc cập nhật dữ liệu.
- Điều hướng giữa các màn hình.
- Validation.
- Điều kiện hiển thị theo vai trò hoặc trạng thái nghiệp vụ.
- Xử lý thành công, thất bại và ngoại lệ.
- Các API được gọi trong từng luồng.
- Cách xác định người dùng hiện tại và quyền thao tác.

Với mỗi chức năng thực tế, ghi nhận:

1. Tên nghiệp vụ.
2. Màn hình và file liên quan.
3. Hành động người dùng có thể thực hiện.
4. Controller và phương thức xử lý.
5. Web Service được sử dụng.
6. API được gọi.
7. Request và response.
8. Quy tắc validation.
9. Điều kiện nghiệp vụ.
10. Quyền thao tác của Nhà tuyển dụng.

Không suy diễn chức năng chỉ từ tên file. Phải xác minh bằng luồng code thực tế.

## V. GIAI ĐOẠN 2 — KHẢO SÁT TOÀN BỘ API

Đọc các API Controller, Service, Repository, DTO và các thành phần liên quan.

Với từng endpoint thực tế, xác định:

- HTTP method.
- Route chính xác.
- Controller và action.
- Request DTO.
- Response DTO.
- Các trường JSON được sử dụng.
- Logic nghiệp vụ.
- Các bảng hoặc entity được truy cập.
- Authentication và authorization.
- Các điều kiện đầu vào.
- Các trạng thái nghiệp vụ.
- Các trường hợp lỗi.
- Các chức năng Web đang sử dụng endpoint.
- Các chức năng có thể tái sử dụng trên Mobile.

Đối chiếu API với chức năng Web đã khảo sát.

Phải kiểm tra cả những endpoint có tên khác nhau nhưng cùng phục vụ một nghiệp vụ. Không tạo endpoint trùng lặp chỉ vì tên API khác tên màn hình.

Nếu Web có chức năng nhưng không tìm thấy API phù hợp, hãy ghi nhận rõ và kiểm tra xem Web có sử dụng một cơ chế xử lý khác hay không trước khi kết luận.

Nếu API tồn tại nhưng chưa xác định được chức năng sử dụng, đánh dấu cần xác minh thay vì mặc định phải đưa lên Mobile.

## VI. GIAI ĐOẠN 3 — KHẢO SÁT VÀ MAPPING CHỨC NĂNG NHÀ TUYỂN DỤNG DỰA TRÊN SOURCE CODE THỰC TẾ

### 3.1. Khảo sát Mobile

Kiểm tra toàn bộ source code Flutter:

- Models.
- Screens.
- Widgets.
- Services.
- Providers, BLoC hoặc cơ chế quản lý trạng thái hiện có.
- Routes và điều hướng.
- API client.
- Authentication và quản lý phiên đăng nhập.
- Các màn hình và luồng nghiệp vụ Nhà tuyển dụng.
- Các nút chưa có xử lý.
- Các chức năng đã gọi API.
- Các chức năng chỉ có giao diện.
- Các chức năng chưa được triển khai.
- Các lỗi ánh xạ dữ liệu hoặc xử lý response.

Không tự ý thay đổi kiến trúc Mobile nếu kiến trúc hiện tại vẫn có thể đáp ứng yêu cầu.

### 3.2. Quy trình mapping

Thực hiện theo trình tự:

**Bước 1:** Khảo sát chức năng thực tế trên Web.

**Bước 2:** Xác định API và logic nghiệp vụ tương ứng.

**Bước 3:** Kiểm tra Mobile đã có chức năng tương ứng hay chưa.

**Bước 4:** Xác định các phần thiếu và dependency cần thiết.

**Bước 5:** Lập bảng mapping đầy đủ.

**Bước 6:** Xác định phạm vi triển khai dựa trên bằng chứng đã thu thập.

**Bước 7:** Tiến hành bổ sung và kiểm thử.

Không bắt đầu tạo chức năng mới trước khi hoàn thành việc xác minh nghiệp vụ và mapping.

### 3.3. Phân loại kết quả mapping

Mỗi chức năng phải được phân loại theo một trong các trạng thái:

- **Đã hoàn thiện trên Mobile:** Đã kết nối API thật và luồng nghiệp vụ đã được xác minh.
- **Có giao diện, chưa tích hợp API:** Màn hình tồn tại nhưng chưa sử dụng đầy đủ dữ liệu thật.
- **Đã tích hợp một phần:** Đã gọi API nhưng còn thiếu thao tác hoặc xử lý.
- **Có trên Web/API, chưa có trên Mobile:** Đã xác minh chức năng tồn tại nhưng Mobile chưa triển khai.
- **Web có, API chưa đáp ứng:** Chức năng tồn tại trên Web nhưng API chưa hỗ trợ đầy đủ.
- **API có, cần xác minh phạm vi Mobile:** Có endpoint nhưng chưa đủ bằng chứng để xác định chức năng thuộc vai trò Nhà tuyển dụng trên Mobile.
- **Chưa đủ bằng chứng:** Chưa thể kết luận chức năng tồn tại hoặc chưa hiểu rõ luồng nghiệp vụ.
- **Không thuộc phạm vi:** Có bằng chứng cho thấy chức năng không áp dụng cho Mobile hoặc không thuộc vai trò Nhà tuyển dụng.

Không đánh dấu hoàn thành chỉ vì đã tìm thấy file hoặc endpoint tương ứng.

### 3.4. Bảng mapping bắt buộc

Tạo hoặc cập nhật file `MOBILE_EMPLOYER_MAPPING.md` tại thư mục gốc project.

Bảng mapping tối thiểu phải có các cột:

| Trường | Nội dung |
|---|---|
| ID | Mã chức năng |
| Nhóm nghiệp vụ | Nhóm được xác minh từ source code |
| Chức năng thực tế | Tên nghiệp vụ đã xác minh |
| Bằng chứng Web | File và phương thức liên quan |
| Bằng chứng API | Endpoint và HTTP method |
| DTO | Request/response thực tế |
| Mobile hiện tại | File hoặc luồng xử lý tương ứng |
| Trạng thái | Kết quả mapping |
| Phần còn thiếu | Nội dung cần bổ sung |
| Hướng xử lý | Phương án triển khai |
| Kiểm thử | Điều kiện xác nhận hoàn thành |

Mỗi dòng phải dẫn đến file, phương thức hoặc endpoint thực tế.

Không điền các chức năng giả định vào bảng như thể chúng đã tồn tại.

### 3.5. Xử lý các chức năng chưa có bằng chứng

Nếu không tìm thấy bằng chứng về một chức năng được gợi ý:

- Không tự tạo chức năng.
- Không tự tạo endpoint.
- Không tự thêm model chỉ để thể hiện rằng đã mapping.
- Không kết luận chức năng là thiếu nếu chưa xác định nó có thuộc phạm vi yêu cầu.
- Ghi nhận là chưa xác minh hoặc không tìm thấy trong source code đã kiểm tra.

Nếu tài liệu nghiệp vụ chính thức yêu cầu chức năng đó nhưng source code chưa có, hãy ghi nhận là yêu cầu chưa được triển khai. Chỉ đề xuất phương án bổ sung sau khi kiểm tra khả năng đáp ứng bằng các cấu trúc và API hiện có.

### 3.6. Điều kiện chuyển sang triển khai

Trước khi sửa code, phải có bảng mapping thể hiện rõ:

- Những chức năng thực tế đã xác minh.
- Những chức năng Mobile đã hoàn thành.
- Những chức năng còn thiếu.
- Những API có thể tái sử dụng.
- Những API còn thiếu.
- Các dependency và rủi ro.
- Những phần cần xác minh thêm.

Sau đó chủ động triển khai những chức năng đã xác định rõ phạm vi. Không cần hỏi tôi xác nhận từng chức năng thông thường đã có đủ bằng chứng.

Nếu một trường hợp mơ hồ có thể ảnh hưởng đến quyền hạn, tiền, hợp đồng hoặc dữ liệu nghiệp vụ, hãy ghi nhận và hỏi tôi trước khi tự quyết định.

## VII. GIAI ĐOẠN 4 — KIỂM TRA CÁC NHÓM NGHIỆP VỤ NHÀ TUYỂN DỤNG

Các nhóm dưới đây chỉ là checklist hỗ trợ khảo sát. **Không mặc định mọi nhóm hoặc mọi chức năng bên trong đều tồn tại.** Chỉ đưa vào phạm vi mapping khi có bằng chứng từ source code hoặc tài liệu nghiệp vụ chính thức.

### A. Tài khoản và hồ sơ Nhà tuyển dụng

Kiểm tra các chức năng thực tế liên quan đến đăng nhập, đăng xuất, xem hồ sơ, cập nhật thông tin và các thao tác tài khoản khác.

### B. Dashboard Nhà tuyển dụng

Kiểm tra các chức năng tổng quan, thống kê và điều hướng thực tế nếu Web/API có triển khai.

### C. Quản lý bài đăng tuyển dụng

Kiểm tra các nghiệp vụ thực tế liên quan đến xem, tạo, sửa, đóng, xóa, tìm kiếm hoặc lọc bài đăng.

### D. Quản lý ứng viên và đơn ứng tuyển

Kiểm tra các chức năng xem hồ sơ ứng viên, danh sách ứng tuyển, chi tiết ứng tuyển và xử lý trạng thái nếu hệ thống hỗ trợ.

### E. Quản lý Freelancer được thuê

Kiểm tra các luồng thuê Freelancer, xác nhận hoặc theo dõi trạng thái nếu có.

### F. Quản lý hợp đồng

Kiểm tra danh sách, chi tiết, trạng thái hợp đồng và các thao tác hợp đồng hiện có.

### G. Ký quỹ và thanh toán

Kiểm tra các chức năng ví, giao dịch, ký quỹ, thanh toán hoặc giải ngân thực tế được cung cấp cho Nhà tuyển dụng.

### H. Theo dõi tiến độ dự án

Kiểm tra các chức năng xem task, trạng thái, deadline, minh chứng và tiến độ theo hợp đồng nếu có.

### I. Bàn giao và nghiệm thu sản phẩm

Kiểm tra các thao tác xem bàn giao, chấp nhận, từ chối hoặc yêu cầu chỉnh sửa nếu nghiệp vụ hiện tại hỗ trợ.

### J. Chat với Freelancer

Kiểm tra các chức năng danh sách chat, lịch sử tin nhắn, gửi tin nhắn và cơ chế nhận tin nhắn hiện có.

### K. Gia hạn deadline, tranh chấp và phản ánh

Kiểm tra các chức năng yêu cầu gia hạn, xử lý tranh chấp và phản ánh nếu tồn tại.

### L. Đánh giá, thông báo và các nghiệp vụ khác

Kiểm tra các chức năng thực tế khác thuộc vai trò Nhà tuyển dụng đã được triển khai trong API/Web.

Không được giới hạn khảo sát ở danh sách trên. Nếu phát hiện thêm chức năng thực tế thuộc vai trò Nhà tuyển dụng, hãy bổ sung vào bảng mapping.

Ngược lại, nếu một nhóm không tồn tại trong source code và không được yêu cầu trong tài liệu nghiệp vụ chính thức, không được tự triển khai chỉ vì nhóm đó xuất hiện trong checklist.

## VIII. GIAI ĐOẠN 5 — BỔ SUNG VÀ HOÀN THIỆN MOBILE

Sau khi hoàn thành mapping, trực tiếp triển khai các phần còn thiếu đã xác minh.

Đối với mỗi chức năng:

1. Xác định endpoint API chính xác.
2. Xác định request và response DTO.
3. Kiểm tra model Flutter hiện có.
4. Tái sử dụng hoặc bổ sung API service.
5. Kết nối màn hình với API thật.
6. Xử lý validation.
7. Xử lý loading, dữ liệu rỗng và lỗi API.
8. Cập nhật giao diện theo kết quả thực tế từ Backend.
9. Kiểm tra quyền truy cập và trạng thái nghiệp vụ.
10. Kiểm thử trước khi chuyển sang chức năng tiếp theo.

Không chỉ tạo giao diện hoặc service riêng lẻ mà không kết nối chúng thành luồng hoạt động hoàn chỉnh.

Không tạo chức năng trùng với chức năng hiện có.

Ưu tiên sửa đổi tối thiểu và giữ nguyên kiến trúc Flutter hiện tại.

## IX. GIAI ĐOẠN 6 — PHÂN QUYỀN VÀ BẢO TOÀN NGHIỆP VỤ

Bắt buộc bảo đảm:

- Nhà tuyển dụng chỉ truy cập dữ liệu thuộc quyền của mình.
- Backend tiếp tục xác minh quyền, không chỉ dựa vào việc ẩn nút trên Mobile.
- Không cho phép truy cập trái phép vào hợp đồng, ứng viên, giao dịch hoặc phòng chat của người dùng khác.
- Không tự thay đổi trạng thái hợp đồng, ứng tuyển, ký quỹ hoặc thanh toán khi Backend chưa xác nhận.
- Không thay đổi logic nghiệp vụ hiện có chỉ để làm Mobile dễ triển khai.
- Không ảnh hưởng đến các chức năng Freelancer đang hoạt động.
- Không kết nối Mobile trực tiếp đến SQL Server.
- Không dùng dữ liệu giả thay cho API thật.

## X. GIAI ĐOẠN 7 — RÀNG BUỘC DATABASE VÀ SOURCE CODE

Không tự ý:

- Thêm hoặc xóa bảng.
- Thêm hoặc xóa cột.
- Đổi tên bảng hoặc cột.
- Thay đổi khóa chính, khóa ngoại hoặc constraint.
- Chạy migration làm thay đổi schema.
- Xóa hoặc sửa dữ liệu nghiệp vụ hiện có.
- Ghi đè toàn bộ file khi chỉ cần bổ sung một phần.
- Xóa code đang hoạt động.
- Tự ý nâng cấp Flutter, Dart hoặc dependency.
- Thay đổi API hoặc Web khi chưa xác minh nhu cầu.

Ưu tiên triển khai Mobile và tái sử dụng API hiện có.

Nếu phát hiện API chưa đáp ứng một chức năng đã được xác minh là cần thiết:

1. Ghi rõ chức năng còn thiếu.
2. Chỉ ra các file và endpoint đã kiểm tra.
3. Xác định liệu API hiện có có thể được tái sử dụng hay mở rộng hay không.
4. Đề xuất thay đổi Backend tối thiểu.
5. Không thay đổi database schema.
6. Không tự triển khai một giải pháp thay thế làm sai nghiệp vụ.
7. Tiếp tục các chức năng độc lập khác trong khi phần bị thiếu được xử lý.

Bảo toàn toàn bộ thay đổi local hiện có trong project.

## XI. GIAI ĐOẠN 8 — KIỂM THỬ

Kiểm thử các chức năng thực tế đã triển khai.

Tùy từng nghiệp vụ, kiểm tra:

- Tải dữ liệu thật từ API.
- Gửi request đúng DTO.
- Xử lý response đúng cấu trúc.
- Cập nhật dữ liệu thành công.
- Hiển thị dữ liệu mới sau khi tải lại.
- Validation và thông báo lỗi.
- Trạng thái loading và danh sách rỗng.
- Phân quyền theo tài khoản.
- Các trường hợp API thất bại.
- Điều hướng giữa các màn hình.
- Tính tương thích với chức năng hiện có.

Chạy `flutter analyze` và build nếu môi trường cho phép.

Phân biệt lỗi tồn tại từ trước với lỗi phát sinh từ thay đổi mới.

Không tự ý sửa những lỗi không liên quan đến phạm vi công việc.

Không tuyên bố build hoặc kiểm thử thành công nếu chưa thực sự chạy và xác nhận kết quả.

## XII. GIAI ĐOẠN 9 — BÁO CÁO VÀ THEO DÕI TIẾN ĐỘ

Cập nhật file `MOBILE_EMPLOYER_MAPPING.md` sau mỗi nhóm chức năng.

Báo cáo cuối cùng phải có:

### 1. Tổng quan

- Các nhóm nghiệp vụ đã kiểm tra.
- Các chức năng thực tế được phát hiện.
- Những chức năng Mobile đã hoàn thiện.
- Những chức năng chỉ có giao diện.
- Những chức năng đã có API nhưng Mobile chưa tích hợp.
- Những chức năng còn thiếu.
- Những chức năng chưa đủ bằng chứng.
- Những chức năng bị chặn do API chưa đáp ứng.

### 2. Danh sách file thay đổi

Với từng file, ghi rõ:

- Đường dẫn.
- Nội dung đã bổ sung hoặc chỉnh sửa.
- Lý do thay đổi.
- API liên quan.
- Chức năng được hoàn thiện.
- Kết quả kiểm thử.

### 3. Trạng thái từng chức năng

Phân loại:

- Hoàn thành và đã kiểm thử.
- Đã triển khai nhưng chưa kiểm thử đầu cuối.
- Chưa triển khai.
- Bị chặn do API hoặc dependency còn thiếu.
- Chưa đủ bằng chứng.
- Không thuộc phạm vi, kèm lý do.

### 4. Các vấn đề còn tồn tại

Liệt kê các API thiếu, lỗi còn tồn tại, phần chưa thể hoàn thiện và nguyên nhân.

Không đánh dấu một chức năng hoàn thành nếu chỉ có giao diện hoặc chưa xác minh luồng dữ liệu thực tế.

## XIII. CHỈ THỊ THỰC HIỆN CUỐI CÙNG

Hãy thực hiện trực tiếp trên source code hiện có của:

- `FreelancerStudent.API`
- `FreelancerStudent.Web`
- `Mobile`

Thực hiện theo quy trình:

**Khảo sát Web → Khảo sát API → Khảo sát Mobile → Xác minh chức năng thực tế → Lập bảng mapping → Xác định phần thiếu → Bổ sung code → Kiểm thử → Cập nhật báo cáo.**

Không chỉ đưa ra hướng dẫn hoặc kế hoạch. Hãy tiếp tục triển khai thực tế sau khi hoàn thành mapping.

**Quy tắc quan trọng nhất:** Không mặc định các chức năng trong danh sách gợi ý là chức năng đã tồn tại hoặc bắt buộc phải tạo. Source code và tài liệu nghiệp vụ chính thức là căn cứ để xác định phạm vi. Danh sách gợi ý chỉ hỗ trợ khảo sát và kiểm tra độ bao phủ.

Không tự suy đoán nghiệp vụ. Không tạo chức năng ngoài phạm vi. Không thay đổi database schema. Không làm hỏng chức năng hiện có.

Nếu chưa xác định được đường dẫn project hoặc gặp xung đột ảnh hưởng đến dữ liệu, quyền hạn hoặc nghiệp vụ quan trọng, hãy báo cáo và hỏi tôi trước khi thực hiện.