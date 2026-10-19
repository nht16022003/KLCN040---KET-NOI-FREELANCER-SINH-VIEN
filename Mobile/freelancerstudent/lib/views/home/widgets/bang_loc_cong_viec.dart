import 'package:flutter/material.dart';

class JobFilterBottomSheet extends StatelessWidget {
  const JobFilterBottomSheet({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: const [
          Text(
            'Bộ lọc công việc',
            style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
          ),
          SizedBox(height: 12),
          Text('Lọc theo mức lương, kỹ năng, hình thức'),
        ],
      ),
    );
  }
}
