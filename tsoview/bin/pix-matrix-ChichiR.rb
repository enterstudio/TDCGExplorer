#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

def combined_matrix(node)
  m = Matrix.identity
  while node != nil
    m = m * node.transformation_matrix
    node = node.parent
  end
  m
end

def dump_matrix(m)
  for row in 0..3
    for col in 0..3
      method_name = "M#{row+1}#{col+1}"
      puts "m.%s = %fF;" % [ method_name, m.send(method_name) ]
    end
  end
end

t1 = Vector3.new(-0.005000, +2.302570, -0.856764)
t2 = Vector3.new(-0.542005, -1.336000, +1.365000)
t3 = Vector3.new(-0.214002, -0.509999, +0.492000)
t4 = Vector3.new(-0.095000, -0.016641, +0.385775)
t5 = Vector3.new(-0.064000, +0.045397, +0.238688)
t5e = Vector3.new(+0.000283, +0.049728, +0.110267)

scaling = Matrix.scaling(1000.0, 1000.0, 1000.0)

# Chichi_Right1 0.0 node ident
src = <<EOT
0 -50.925  62.582  459.349
1  14.189 993.833 -129.007
2  -0.04049 2.23481 -0.43389
3 838.939   2.235   92.417
EOT

p1 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m1 = p1
puts "Chichi_Right1"
dump_matrix m1

# Chichi_Right2 0.0 node ident
src = <<EOT
0  -8.133 -22.132 530.147
1  10.684 935.408  41.513
2  -0.46235 1.13679 0.3203
3 789.334  -8.771  11.320
EOT

p2 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m2 = p2 * Matrix.invert(p1)
puts "Chichi_Right2"
dump_matrix m2

# Chichi_Right3 0.0 node ident
src = <<EOT
0 -10.602 -29.564 688.564
1   7.567 688.676  30.893
2  -0.641   0.651   0.558
3 687.980  -7.987  10.148
EOT

p3 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m3 = p3 * Matrix.invert(p2)
puts "Chichi_Right3"
dump_matrix m3

# Chichi_Right4 0.0 node ident
src = <<EOT
0 -15.173 -43.241 999.752
1  11.207 999.585  44.866
2  -0.710   0.629   0.822
3 999.112 -11.913  14.746
EOT

p4 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m4 = p4 * Matrix.invert(p3)
puts "Chichi_Right4"
dump_matrix m4

# Chichi_Right5 0.0 node ident
src = <<EOT
0 -15.240 -43.206 999.992
1  11.140 999.621  45.106
2  -0.777   0.664   1.061
3 999.045 -11.878  14.986
EOT

p5 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m5 = p5 * Matrix.invert(p4)
puts "Chichi_Right5"
dump_matrix m5

# Chichi_Right5_end 0.0 node ident
src = <<EOT
0 -15.240 -43.161 1000.104 
1  11.140 999.666   45.218
2  -0.778   0.709    1.174
3 999.044 -11.833   15.098
EOT

p5e = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m5e = p5e * Matrix.invert(p5)
puts "Chichi_Right5_end"
dump_matrix m5e
