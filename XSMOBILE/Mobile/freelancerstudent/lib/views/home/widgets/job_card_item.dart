import 'package:flutter/material.dart';

class JobCardItem extends StatelessWidget {
  const JobCardItem({super.key});

  @override
  Widget build(BuildContext context) {
    return Card(
      child: ListTile(
        title: const Text('UI/UX Designer'),
        subtitle: const Text('Dự án mobile app • 10 triệu'),
        trailing: IconButton(
          onPressed: () {},
          icon: const Icon(Icons.arrow_forward_ios),
        ),
      ),
    );
  }
}
