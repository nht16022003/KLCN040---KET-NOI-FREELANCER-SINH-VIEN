Đã hoàn thành xong chức năng và giao diện của đăng tin tuyển dụng và quản lý hồ sơ nhà tuyển dụng

Trong database đã thêm cho bảng user thuộc tính avatar để khi bấm vào Thiết lập tài khoản mục avatar có thể up hình
ALTER TABLE Users
ADD avatarUrl NVARCHAR(500) NULL;

Phần đăng tin tuyển dụng, chỗ phí đăng bài tạm thời để = 0, sau này admin xong chức năng chỉnh phí đăng bài rồi quay lại chỉnh


Khi mới đăng nhập vào sẽ ở giao diện index(đang nghi vấn là trang của "Đăng tin mới"). Sau khi bấm đăng bài tuyển dụng xong sẽ qua giao diện quản lý tin tuyển dụng, lúc đăng bài tuyển dụng bấm quay lại cũng về trang giao diện quản lý tin tuyển dụng. Còn trang giao diện quản lý tin tuyển dụng hiện tại chưa có liên kết nên bấm vào không ra cái gì. Nói chung đã xong được UI và chức năng của Đăng bài tuyển dụng và thiết lập tài khoản rồi, có thiết lập tài khoản là OK thôi còn cái đăng bài tuyển dụng đang gặp vấn đề sau khi bấm quay lại hoặc đăng bài