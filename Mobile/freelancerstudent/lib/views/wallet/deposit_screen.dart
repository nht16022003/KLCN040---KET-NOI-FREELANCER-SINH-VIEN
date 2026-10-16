import 'package:flutter/material.dart';

class DepositScreen extends StatelessWidget {
  const DepositScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Nạp tiền')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            TextField(
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(labelText: 'Số tiền cần nạp'),
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<String>(
              decoration: const InputDecoration(
                labelText: 'Phương thức thanh toán',
              ),
              items: const [
                DropdownMenuItem(value: 'VNPAY', child: Text('VNPAY')),
                DropdownMenuItem(value: 'MoMo', child: Text('MoMo')),
              ],
              onChanged: (_) {},
            ),
            const SizedBox(height: 20),
            FilledButton(onPressed: () {}, child: const Text('Thanh toán')),
          ],
        ),
      ),
    );
  }
}
