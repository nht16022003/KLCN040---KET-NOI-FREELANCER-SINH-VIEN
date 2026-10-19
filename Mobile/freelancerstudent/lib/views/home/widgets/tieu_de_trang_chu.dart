import 'package:flutter/material.dart';

class HomeHeader extends StatelessWidget {
  const HomeHeader({super.key});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: const [
        CircleAvatar(child: Icon(Icons.person)),
        SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Xin chào, Freelancer',
                style: TextStyle(fontWeight: FontWeight.bold),
              ),
              Text('Hãy bắt đầu ngày mới nào!'),
            ],
          ),
        ),
      ],
    );
  }
}
