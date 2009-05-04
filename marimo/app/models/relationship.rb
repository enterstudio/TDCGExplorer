class Relationship < ActiveRecord::Base
  belongs_to :from, :class_name => 'Arc'
  belongs_to :to, :class_name => 'Arc'

  def self.kind_collection
    [['������e', 1], ['�V��', 2], ['�O��', 3]]
  end

  def kind_caption
    case kind
    when 1
      '������e'
    when 2
      '�V��'
    when 3
      '�O��'
    end
  end

  def rev_kind_caption
    case kind
    when 1
      '������e'
    when 2
      '����'
    when 3
      '��'
    end
  end

  def to_code
    to ? to.code : nil
  end

  def to_code=(to_code)
    to = Arc.find_by_code(to_code)
    self.to_id = to ? to.id : nil
  end

  def should_destroy?
    to_id.nil?
  end
end