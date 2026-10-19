import 'package:flutter/material.dart';

class JobSearchBar extends StatelessWidget {
  const JobSearchBar({super.key});

  @override
  Widget build(BuildContext context) {
    return const TextField(
      decoration: InputDecoration(
        hintText: 'Tìm kiếm công việc...',
        prefixIcon: Icon(Icons.search),
        border: OutlineInputBorder(),
      ),
    );
  }
}
